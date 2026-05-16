using UnityEngine;

public class HeroView : CombatantView
{
    public void Setup(HeroData heroData, int currentHealth)
    {
        SetupBase(heroData.Health, heroData.Image, currentHealth);
    }
}
