using System;
using UnityEngine;

public class OnEnemyAttackCondition : ArtifactCondition
{
    public override bool SubConditionIsMet(GameAction gameAction)
    {
        // if attacker is above x health, ...можно что-то добавить
        return true;
    }

    public override void SubscribeCondition(Action<GameAction> reaction)
    {
        ActionSystem.SubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }

    public override void UnsubscribeCondition(Action<GameAction> reaction)
    {
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(reaction, reactionTiming);
    }
}
