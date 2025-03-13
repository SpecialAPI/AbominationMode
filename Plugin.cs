using BepInEx;
using BrutalAPI;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AbominationMode
{
    [BepInPlugin(GUID, "Abomination Mode", "1.0.0")]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "SpecialAPI.AbominationMode";

        public static Dictionary<string, int> EnemiesWithAbom = new()
        {
            // basegame
            ["OneManBand_EN"] = 1,
            ["UnfinishedHeir_BOSS"] = 1,
            
            // modded
            ["VanishingHands_EN"] = 2,
        };
        public static int AddAbom = 1;

        public static MethodInfo aa_ne_a = AccessTools.Method(typeof(Plugin), nameof(AddAbomination_NewEnemy_Add));
        public static MethodInfo aa_u_a = AccessTools.Method(typeof(Plugin), nameof(AddAbomination_Unbox_Add));

        public void Awake()
        {
            var harmony = new Harmony(GUID);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(EnemyCombat), MethodType.Constructor, typeof(int), typeof(int), typeof(EnemySO), typeof(bool), typeof(int))]
        [HarmonyILManipulator]
        public static void AddAbomination_NewEnemy_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.DefaultPassiveAbilityInitialization))))
                return;

            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, aa_ne_a);
        }

        public static void AddAbomination_NewEnemy_Add(EnemyCombat en)
        {
            if (en == null)
                return;

            if (en.Enemy == null || string.IsNullOrEmpty(en.Enemy.name) || !EnemiesWithAbom.TryGetValue(en.Enemy.name, out var origAbom))
                origAbom = 0;

            if (en.TryGetPassiveAbility(PassiveType_GameIDs.Abomination.ToString(), out var exist))
            {
                if (origAbom > 0)
                {
                    en.PassiveAbilities.Remove(exist);
                    exist.OnTriggerDettached(en);

                    if (en.ContainsPassiveAbility(PassiveType_GameIDs.Decay.ToString()))
                        return; // WTF?
                }
                else
                    return;
            }

            var abomAmt = origAbom + AddAbom;
            var abom = Passives.AbominationGenerator(abomAmt);
            en.PassiveAbilities.Add(abom);
            abom.OnTriggerAttached(en);
        }
        
        public static void AddAbomination(EnemyCombat en)
        {
            if (en.Enemy == null || string.IsNullOrEmpty(en.Enemy.name) || !EnemiesWithAbom.TryGetValue(en.Enemy.name, out var origAbom))
                origAbom = 0;

            var abomAmt = origAbom + AddAbom;
            var abom = Passives.AbominationGenerator(abomAmt);
            en.AddPassiveAbility(abom);
        }

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

            while (crs.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt<EnemyCombat>(nameof(EnemyCombat.ConnectPassives))))
            {
                crs.Emit(OpCodes.Ldloc_3);
                crs.Emit(OpCodes.Call, aa_u_a);
            }
        }

        public static void AddAbomination_Unbox_Add(EnemyCombat enm)
        {
            AddAbomination(enm);
        }
    }
}
