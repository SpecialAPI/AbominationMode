using BrutalAPI;
using HarmonyLib;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using AbominationMode.Tools;

namespace AbominationMode
{
    [HarmonyPatch]
    public static class AbominationModePatches
    {
        public static readonly PassiveWithCountInfo AbominationInfo = new("Abomination", false, true);

        [HarmonyPatch(typeof(EnemyCombat), nameof(EnemyCombat.TransformEnemy))]
        [HarmonyPostfix]
        public static void AddAbomination_PostTransform_Postfix(EnemyCombat __instance)
        {
            AddAbomination(__instance);
        }

        [HarmonyPatch(typeof(CombatStats), nameof(CombatStats.TryUnboxEnemy))]
        [HarmonyILManipulator]
        public static void AddAbomination_Unbox_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            foreach(var m in crs.MatchAfter(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
            {
                crs.Emit(OpCodes.Ldloc_3);
                crs.EmitStaticDelegate(AddAbomination_Unbox_Add);
            }
        }

        public static void AddAbomination_Unbox_Add(EnemyCombat enm)
        {
            AddAbomination(enm);
        }

        [HarmonyPatch(typeof(EnemyCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(EnemySO), typeof(bool), typeof(int))]
        [HarmonyILManipulator]
        public static void AddAbomination_NewEnemy_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.JumpToNext(x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.EmitStaticDelegate(AddAbomination_NewEnemy_Add);
        }

        public static void AddAbomination_NewEnemy_Add(EnemyCombat en)
        {
            if(!TryGetAbominationAmount(en, out var abomAmt, false, false))
                return;

            var abom = Passives.AbominationGenerator(abomAmt);
            en.PassiveAbilities.Add(abom);
            abom.OnTriggerAttached(en);
        }

        public static void AddAbomination(EnemyCombat en)
        {
            if (!TryGetAbominationAmount(en, out var abomAmt, true, true))
                return;

            var abom = Passives.AbominationGenerator(abomAmt);
            en.AddPassiveAbility(abom);
        }

        public static bool TryGetAbominationAmount(EnemyCombat en, out int amount, bool disconnect, bool doUIChange)
        {
            amount = 0;
            if (en == null)
                return false;

            var addAmount = ModConfig.AbominationAmount;
            if (addAmount == 0)
                return false;

            var origAbom = 0;
            if (en.TryGetPassiveAbility(PassiveType_GameIDs.Abomination.ToString(), out var exist))
            {
                if (AbominationInfo.TryGetCount(exist, out var count))
                {
                    origAbom = count;

                    en.PassiveAbilities.Remove(exist);
                    exist.OnTriggerDettached(en);
                    if(disconnect)
                        exist.OnPassiveDisconnected(en);

                    if (en.ContainsPassiveAbility(PassiveType_GameIDs.Abomination.ToString()))
                    {
                        if(doUIChange)
                            CombatManager.Instance.AddUIAction(new EnemyPassiveAbilityChangeUIAction(en.ID, [.. en.PassiveAbilities]));

                        return false;
                    }
                }
                else
                    return false;
            }

            amount = origAbom + addAmount;
            return true;
        }
    }
}
