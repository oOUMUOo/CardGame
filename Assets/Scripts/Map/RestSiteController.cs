using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Узел отдыха: по нажатию восстанавливает текущее HP + 30% от максимального (не выше максимума).
/// </summary>
public class RestSiteController : MonoBehaviour
{
    private const float HealFractionOfMax = 0.3f;

    [Header("UI")]
    [SerializeField] private Button restButton;
    [SerializeField] private Button backButton;
    [SerializeField] private MainPanelUI mainPanel;

    [Header("Routes")]
    [SerializeField] private MapSceneRoutesData sceneRoutes;

    [Header("Data")]
    [SerializeField] private HeroData heroData;

    [Tooltip("Если включено — после отдыха кнопка становится неактивной (один отдых за узел).")]
    [SerializeField] private bool disableRestAfterUse = true;

    private bool _restUsed;

    private void Awake()
    {
        if (restButton != null)
        {
            restButton.onClick.AddListener(OnClickRest);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnClickBackToMap);
        }
    }

    private void Start()
    {
        ApplyRestButtonState();
    }

    private void OnDestroy()
    {
        if (restButton != null)
        {
            restButton.onClick.RemoveListener(OnClickRest);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnClickBackToMap);
        }
    }

    private void OnClickBackToMap()
    {
        if (sceneRoutes == null)
        {
            Debug.LogError("RestSiteController: назначьте MapSceneRoutesData.");
            return;
        }

        MapEncounterBridge.CompleteEncounterAndLoadMap(sceneRoutes);
    }

    private void ApplyRestButtonState()
    {
        if (restButton != null && disableRestAfterUse && _restUsed)
        {
            restButton.interactable = false;
        }
    }

    private void OnClickRest()
    {
        if (heroData == null || _restUsed && disableRestAfterUse)
        {
            return;
        }

        int maxHp = heroData.Health;
        int current = ResolveCurrentHealth(maxHp);

        int heal = Mathf.RoundToInt(maxHp * HealFractionOfMax);
        int newHealth = Mathf.Min(maxHp, current + heal);

        RunSaveSystem.SaveHeroHealth(newHealth);

        if (disableRestAfterUse)
        {
            _restUsed = true;
            if (restButton != null)
            {
                restButton.interactable = false;
            }
        }

        if (mainPanel != null)
        {
            mainPanel.Refresh();
        }
    }

    private static int ResolveCurrentHealth(int maxHp)
    {
        int currentHealth = maxHp > 0 ? maxHp : 0;

        if (!RunSaveSystem.TryLoadHeroHealth(out int savedHealth))
        {
            return currentHealth;
        }

        if (maxHp > 0)
        {
            return Mathf.Clamp(savedHealth, 0, maxHp);
        }

        return Mathf.Max(0, savedHealth);
    }
}
