using BepInEx;
using BepInEx.Configuration;
using BrutalAPI;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AbominationMode
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "SpecialAPI.AbominationMode";
        public const string MOD_NAME = "Abomination Mode";
        public const string MOD_VERSION = "1.1.0";
        public const string MOD_PREFIX = "AbominationMode";

        public static readonly Harmony HarmonyInstance = new(MOD_GUID);

        public void Awake()
        {
            ModConfig.Config = Config;
            ModConfig.Init();

            AlchemicalAbominationModePatches.Init();

            HarmonyInstance.PatchAll();
        }
    }
}
