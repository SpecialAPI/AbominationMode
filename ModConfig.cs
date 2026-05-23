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

        public static int AbominationAmount => AbominationAmountConfig.Value;

        public static void Init()
        {
            AbominationAmountConfig = Config.Bind("AbominationMode", "AbominationAmount", 1, "The amount of Abomination that will be given to enemies.");
        }
    }
}
