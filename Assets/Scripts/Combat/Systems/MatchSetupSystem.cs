using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private ArtifactData artifactData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(enemyDatas);
        CardSystem.Instance.Setup(heroData.Deck);
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
