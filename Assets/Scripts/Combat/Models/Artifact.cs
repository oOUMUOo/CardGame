using System.Collections.Generic;
using UnityEngine;

public class Artifact
{
    public Sprite Image => data.Image;
    private readonly ArtifactData data;
    private readonly ArtifactCondition condition;
    private readonly AutoTargetEffect effect;

    public Artifact(ArtifactData artifactData)
    {
        data = artifactData;
        condition = data.ArtifactCondition;
        effect = data.AutoTargetEffect;
    }

    public void OnAdd()
    {
        condition.SubscribeCondition(Reaction);
    }
    public void OnRemove()
    {
        condition.UnsubscribeCondition(Reaction);
    }

    private void Reaction(GameAction gameAction)
    {
        if (condition.SubConditionIsMet(gameAction))
        {
            List<CombatantView> targets = new();
            if (data.UseActionCasterAsTarget && gameAction is IHaveCaster haveCaster)
            {
                targets.Add(haveCaster.Caster);
            }
            if (data.UseAutoTarget)
            {
                targets.AddRange(effect.TargetMode.GetTargets());
            }
            GameAction artifactEffectAction = effect.Effect.GetGameAction(targets, HeroSystem.Instance.HeroView);
            ActionSystem.Instance.AddReaction(artifactEffectAction);
        }
    }
}
