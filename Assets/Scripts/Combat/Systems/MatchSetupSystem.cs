using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private DeckBuildCatalog deckBuildCatalog;
    [SerializeField] private ArtifactData artifactData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(enemyDatas);
        var deck = DeckPresetSaveSystem.ResolveDeckOrFallback(deckBuildCatalog, heroData);
        if (deckBuildCatalog != null)
        {
            foreach (var name in RunDeckBonusCardsSystem.GetNames())
            {
                if (deckBuildCatalog.TryGetByAssetName(name, out var bonus))
                {
                    deck.Add(bonus);
                }
            }
        }

        CardSystem.Instance.Setup(deck);
        ArtifactSystem.Instance.AddArtifact(new Artifact(artifactData));
        //
        DrawCardsGA drawCardsGA = new(5);
        ActionSystem.Instance.Perform(drawCardsGA);

        // RefillManaGA refillManaGA = new();

        // ActionSystem.Instance.Perform(refillManaGA, ()=>
        // {
        //     DrawCardsGA drawCardsGA = new(5);
        //     ActionSystem.Instance.Perform(drawCardsGA);
        // }); 
    
    }
}
