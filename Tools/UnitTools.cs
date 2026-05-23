using System;
using System.Collections.Generic;
using System.Text;

namespace AbominationMode.Tools
{
    public static class UnitTools
    {
        public static bool TryPerformRandomAbilityWorksOnEnemies(this IUnit u, AbilitySO ability)
        {
            if(u is EnemyCombat ec)
            {
                CombatManager.Instance.AddSubAction(new ShowAttackInformationUIAction(ec.ID, ec.IsUnitCharacter, ability.GetAbilityLocData().text));
                CombatManager.Instance.AddSubAction(new PlayAbilityAnimationAction(ability.visuals, ability.animationTarget, ec));
                CombatManager.Instance.AddSubAction(new EffectAction(ability.effects, ec));

                return true;
            }
            else
                return u.TryPerformRandomAbility(ability);
        }
    }
}
