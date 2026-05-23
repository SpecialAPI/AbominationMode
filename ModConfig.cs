using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbominationMode
{
    public static class ModConfig
    {
        public static ConfigFile Config;

        public static ConfigEntry<int> AbominationAmountConfig;
        public static ConfigEntry<bool> AlchemicalAbominationModeEnabledConfig;
        public static ConfigEntry<int> AlchemicalAbominationAbilityWeightConfig;

        public static int AbominationAmount => AbominationAmountConfig.Value;
        public static bool AlchemicalAbominationModeEnabled => AlchemicalAbominationModeEnabledConfig.Value;
        public static int AlchemicalAbominationAbilityWeight => AlchemicalAbominationAbilityWeightConfig.Value;

        public static void Init()
        {
            AbominationAmountConfig = Config.Bind("AbominationMode", "AbominationAmount", 1, "The amount of Abomination that will be given to enemies.");
            AlchemicalAbominationModeEnabledConfig = Config.Bind("AlchemicalAbominationMode", "Enabled", false, "If true, all enemies will get Alchemical Abomination as an extra ability.");
            AlchemicalAbominationAbilityWeightConfig = Config.Bind("AlchemicalAbominationMode", "AbilityWeight", 5, "Determines how common the added Alchemical Abomination ability is. Does nothing if Alchemical Abomination Mode is not enabled.");
        }
    }
}
