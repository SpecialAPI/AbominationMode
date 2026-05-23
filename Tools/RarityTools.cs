using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AbominationMode.Tools
{
    public static class RarityTools
    {
        private static readonly Dictionary<int, RaritySO> rarities = [];

        public static RaritySO GetOrCreateRarity(int rarityValue)
        {
            if(rarities.TryGetValue(rarityValue, out RaritySO rarity))
                return rarity;

            rarity = ScriptableObject.CreateInstance<RaritySO>();
            rarity.rarityValue = rarityValue;
            rarity.canBeRerolled = true;
            rarities[rarityValue] = rarity;

            return rarity;
        }
    }
}
