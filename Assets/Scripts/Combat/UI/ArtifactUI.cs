using UnityEngine;
using UnityEngine.UI;

public class ArtifactUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public Artifact Artifact {get; private set;}

    public void Setup(Artifact artifact)
    {
        Artifact = artifact;
        image.sprite = artifact.Image;
    }
}
