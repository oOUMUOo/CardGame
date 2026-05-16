using System.Collections.Generic;
using UnityEngine;

public class ArtifactSystem : Singleton<ArtifactSystem>
{
    [SerializeField] private ArtifactsUI artifactsUI;

    private readonly List<Artifact> artifacts = new();

    private void OnDisable()
    {
        // Важно снимать подписки артефактов перед выгрузкой сцены боя,
        // иначе реакции копятся между боями.
        foreach (var artifact in artifacts)
        {
            artifact.OnRemove();
        }
        artifacts.Clear();
    }

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
