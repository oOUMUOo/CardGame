using SerializeReferenceEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Artifact")]

public class ArtifactData : ScriptableObject
{
    [field: SerializeField] public Sprite Image {get; private set;}
    [field: SerializeReference, SR] public ArtifactCondition ArtifactCondition {get; private set;}
    [field: SerializeReference, SR] public AutoTargetEffect AutoTargetEffect {get; private set;}
    [field: SerializeField] public bool UseAutoTarget {get; private set;} = true;
    [field: SerializeField] public bool UseActionCasterAsTarget {get; private set;} = false;
}
