using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatVictoryUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Rewards")]
    [SerializeField] private TMP_Text goldRewardText;
    [SerializeField] private TMP_Text additionalRewardText;
    [SerializeField] private int goldReward = 25;
    [SerializeField] private string additionalReward = "Choose 1 card";

    [Header("Navigation")]
    [SerializeField] private MapSceneRoutesData sceneRoutes;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (root == null)
        {
            root = gameObject;
        }

        if (continueButton == null)
        {
            continueButton = GetComponent<Button>();
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnClickContinue);
            continueButton.onClick.AddListener(OnClickContinue);
        }

        Hide();
    }

    public void Show()
    {
        if (root != null)
        {
            root.SetActive(true);
            root.transform.localScale = Vector3.one;
        }

        if (goldRewardText != null)
        {
            goldRewardText.text = $"+{goldReward} Gold";
        }

        if (additionalRewardText != null)
        {
            additionalRewardText.text = additionalReward;
        }
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public void OnClickContinue()
    {
        if (sceneRoutes == null)
        {
            Debug.LogError("CombatVictoryUI: MapSceneRoutesData is not assigned.");
            return;
        }

        MapEncounterBridge.CompleteEncounterAndLoadMap(sceneRoutes);
    }
}
