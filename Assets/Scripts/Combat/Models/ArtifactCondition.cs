using System;
using UnityEngine;

public abstract class ArtifactCondition
{
    [SerializeField] protected ReactionTiming reactionTiming;

    public abstract void SubscribeCondition(Action<GameAction> reaction);
    public abstract void UnsubscribeCondition(Action<GameAction> reaction);
    public abstract bool SubConditionIsMet(GameAction gameAction);
}
