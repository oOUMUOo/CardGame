using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [field: SerializeField] public HeroView HeroView {get; private set;}
    [SerializeField] private string baseSceneName = "BaseScene";
    [SerializeField] private CombatDeathPanelUI deathPanel;

    private bool runEnded;

    void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(HeroData heroData)
    {
        runEnded = false;

        int startingHealth = heroData.Health;
        if (RunSaveSystem.TryLoadHeroHealth(out int savedHealth))
        {
            startingHealth = Mathf.Clamp(savedHealth, 1, heroData.Health);
        }

        HeroView.Setup(heroData, startingHealth);
        SaveCurrentHealth();
    }

    public void SaveCurrentHealth()
    {
        if (HeroView == null)
        {
            return;
        }

        RunSaveSystem.SaveHeroHealth(HeroView.CurrentHealth);
    }

    public void HandleHeroDamaged()
    {
        if (runEnded || HeroView == null)
        {
            return;
        }

        if (HeroView.CurrentHealth <= 0)
        {
            EndRun();
            return;
        }

        SaveCurrentHealth();
    }

    private void EndRun()
    {
        if (runEnded)
        {
            return;
        }

        runEnded = true;
        SaveCurrentHealth();

        if (deathPanel != null)
        {
            deathPanel.Show();
            return;
        }

        if (!RunAbortUtility.TryQuitRunAndLoadScene(baseSceneName))
        {
            runEnded = false;
        }
    }

    //Reactions

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        int burnStacks = HeroView.GetStatusEffectStacks(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            ApplyBurnGA applyBurnGA = new(burnStacks, HeroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
}
