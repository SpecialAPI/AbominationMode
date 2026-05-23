using AbominationMode.Tools;
using BrutalAPI;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Priority = BrutalAPI.Priority;

namespace AbominationMode
{
    [HarmonyPatch]
    public static class AlchemicalAbominationModePatches
    {
        public static AbilitySO EvilAlchemicalAbomination;

        public static void Init()
        {
            var evilAlchemicalAbomAB = new Ability($"{Plugin.MOD_PREFIX}_EvilAlchemicalAbomination_A")
            {
                Name = "Alchemical Abomination",
                Description = "Performs 3 random party member abilities back-to-back.",
                Effects =
                [
                    Effects.GenerateEffect(ScriptableObject.CreateInstance<PerformRandomAbilityFromCharacterWorksOnEnemiesEffect>(), 3)
                ],
                Rarity = Rarity.Uncommon,
                Priority = Priority.Normal,
                AbilitySprite = LoadedDBsHandler.EnemyDB.DefaultAbilitySprite
            };
            evilAlchemicalAbomAB.AddIntentsToTarget(Targeting.Slot_SelfSlot, [IntentType_GameIDs.Misc_Hidden.ToString()]);


            EvilAlchemicalAbomination = evilAlchemicalAbomAB.ability;
            LoadedAssetsHandler.AddExternalEnemyAbility(EvilAlchemicalAbomination.name, EvilAlchemicalAbomination);
        }

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.SetUpDefaultAbilities))]
        [HarmonyILManipulator]
        public static void AddAlchemicalAbomination_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpBeforeNext(x => x.MatchCallOrCallvirt<EnemyCombat>($"get_{nameof(EnemyCombat.ExtraAbilities)}")))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAlchemicalAbomination_Add);
        }

        public static List<ExtraAbilityInfo> AddAlchemicalAbomination_Add(List<ExtraAbilityInfo> curr, EnemyCombat en)
        {
            if(!ModConfig.AlchemicalAbominationModeEnabled)
                return curr;

            var alchemicalAbom = EvilAlchemicalAbomination;
            if(alchemicalAbom == null)
                return curr;

            var rarity = Mathf.Max(0, ModConfig.AlchemicalAbominationAbilityWeight);
            en.Abilities.Add(new(alchemicalAbom, RarityTools.GetOrCreateRarity(rarity)));
            return curr;
        }

        [HarmonyPatch(typeof(ExtraAbilityInfo), MethodType.Constructor, typeof(CharacterAbility))]
        [HarmonyPostfix]
        public static void FixExtraAbilityRarity_Postfix(ExtraAbilityInfo __instance)
        {
            if (!ModConfig.AlchemicalAbominationModeEnabled)
                return;

            __instance.rarity ??= Rarity.Common;
        }
    }
}
