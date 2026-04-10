using UnityEngine;

public class MapUI : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    
    public void ShowMap()
    {
        mapPanel.SetActive(true);
    }
    
    public void HideMap()
    {
        mapPanel.SetActive(false);
    }
    
    public void ToggleMap()
    {
        mapPanel.SetActive(!mapPanel.activeSelf);
    }
}
