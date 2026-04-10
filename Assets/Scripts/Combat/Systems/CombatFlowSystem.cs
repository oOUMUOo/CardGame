using System.Collections;
using UnityEngine;

public class CombatFlowSystem : MonoBehaviour
{
    [SerializeField] private CombatVictoryUI combatVictoryUI;

    private void Awake()
    {
        if (combatVictoryUI == null)
        {
            combatVictoryUI = FindFirstObjectByType<CombatVictoryUI>();
        }
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<CombatVictoryGA>(CombatVictoryPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<CombatVictoryGA>();
    }

    private IEnumerator CombatVictoryPerformer(CombatVictoryGA combatVictoryGA)
    {
        if (combatVictoryUI == null)
        {
            combatVictoryUI = FindFirstObjectByType<CombatVictoryUI>();
        }

        if (combatVictoryUI != null)
        {
            combatVictoryUI.Show();
        }
        else
        {
            Debug.LogError("CombatFlowSystem: CombatVictoryUI reference is missing.");
        }

        yield return null;
    }
}
