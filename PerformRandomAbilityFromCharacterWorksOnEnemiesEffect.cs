using AbominationMode.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbominationMode
{
    public class PerformRandomAbilityFromCharacterWorksOnEnemiesEffect : EffectSO
    {
        public override bool PerformEffect(CombatStats stats, IUnit caster, TargetSlotInfo[] targets, bool areTargetSlots, int entryVariable, out int exitAmount)
        {
            exitAmount = 0;

            var db = LoadedDBsHandler.AbilityDB;
            if (db == null)
                return false;

            foreach (var randomCharacterAbility in db.GetRandomCharacterAbilities(entryVariable))
            {
                if (!caster.TryPerformRandomAbilityWorksOnEnemies(randomCharacterAbility))
                    continue;

                exitAmount++;
            }
            return exitAmount > 0;
        }
    }
}
