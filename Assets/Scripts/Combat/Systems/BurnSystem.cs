using System.Collections;
using UnityEngine;

public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();
    }

    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)
    {
        CombatantView target = applyBurnGA.Target;
        if (target == null)
        {
            yield break;
        }

        Instantiate(burnVFX, target.transform.position, Quaternion.identity);
        target.Damage(applyBurnGA.BurnDamage);
        if (target is HeroView)
        {
            HeroSystem.Instance.HandleHeroDamaged();
        }

        if (target == null)
        {
            yield break;
        }

        target.RemoveStatusEffect(StatusEffectType.BURN, 1);

        if (target.CurrentHealth <= 0)
        {
            if (target is EnemyView enemyView)
            {
                KillEnemyGA killEnemyGA = new(enemyView);
                ActionSystem.Instance.AddReaction(killEnemyGA);
            }
            else
            {
                HeroSystem.Instance.HandleHeroDamaged();
            }
        }

        yield return new WaitForSeconds(1f);
    }
}
