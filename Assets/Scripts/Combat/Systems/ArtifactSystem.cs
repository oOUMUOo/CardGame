using System.Collections.Generic;
using UnityEngine;

public class ArtifactSystem : Singleton<ArtifactSystem>
{
    [SerializeField] private ArtifactsUI artifactsUI;

    private readonly List<Artifact> artifacts = new();

    public void AddArtifact(Artifact artifact)
    {
        artifacts.Add(artifact);
        artifactsUI.AddArtifactUI(artifact);
        artifact.OnAdd();
    }
    
    public void RemoveArtifact(Artifact artifact)
    {
        artifacts.Remove(artifact);
        artifactsUI.RemoveArtifactUI(artifact);
        artifact.OnRemove();
    }
}
