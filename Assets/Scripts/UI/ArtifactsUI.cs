using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtifactsUI : MonoBehaviour
{
    [SerializeField] private ArtifactUI artifactUIPrefab;

    private readonly List<ArtifactUI> artifactUIs = new();

    public void AddArtifactUI(Artifact artifact)
    {
        ArtifactUI artifactUI = Instantiate(artifactUIPrefab, transform);
        artifactUI.Setup(artifact);
        artifactUIs.Add(artifactUI);
    }

    public void RemoveArtifactUI(Artifact artifact)
    {
        ArtifactUI artifactUI = artifactUIs.Where(aui => aui.Artifact == artifact).FirstOrDefault();
        if (artifactUI != null)
        {
            artifactUIs.Remove(artifactUI);
            Destroy(artifactUI.gameObject);
        }
    }
}
