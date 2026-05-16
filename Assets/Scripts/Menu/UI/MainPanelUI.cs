using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainPanelUI : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private HeroData heroData;

    [Header("Combat — настройки / выход (опционально)")]
    [SerializeField] private Button settingsOpenButton;
    [SerializeField] private GameObject settingsPanelRoot;
    [SerializeField] private Button settingsCloseButton;

    [SerializeField] private string mainMenuSceneName = "MenuScene";
    [SerializeField] private string baseSceneName = "BaseScene";
    [SerializeField] private Button goToMainMenuButton;
    [SerializeField] private Button openReturnToBaseConfirmButton;

    [SerializeField] private GameObject returnToBaseConfirmRoot;
    [SerializeField] private Button returnToBaseConfirmYesButton;
    [SerializeField] private Button returnToBaseConfirmNoButton;

    [SerializeField] private TMP_Text errorTextOutput;

    private bool _blockingInteractions;

    private void Awake()
    {
        if (settingsPanelRoot != null && settingsPanelRoot.activeSelf)
        {
            settingsPanelRoot.SetActive(false);
        }

        CloseConfirmReturnToBase();
        SubscribeButton(settingsOpenButton, OnClickOpenSettings);
        SubscribeButton(settingsCloseButton, () => CloseSettings(closeConfirmToo: true));
        SubscribeButton(goToMainMenuButton, OnClickMainMenu);
        SubscribeButton(openReturnToBaseConfirmButton, OnClickOpenConfirmReturnBase);
        SubscribeButton(returnToBaseConfirmYesButton, OnClickConfirmYesReturnBase);
        SubscribeButton(returnToBaseConfirmNoButton, CloseConfirmReturnToBase);
    }

    private static void SubscribeButton(Button btn, UnityAction action)
    {
        if (btn != null && action != null)
        {
            btn.onClick.AddListener(action);
        }
    }

    private void OnDestroy()
    {
        if (_blockingInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PopUIBlocker();
            _blockingInteractions = false;
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        RefreshHealth();
        RefreshGold();
    }

    private void RefreshHealth()
    {
        int maxHealth = heroData != null ? heroData.Health : 0;
        int currentHealth = maxHealth;

        if (RunSaveSystem.TryLoadHeroHealth(out int savedHealth))
        {
            if (maxHealth > 0)
            {
                currentHealth = Mathf.Clamp(savedHealth, 0, maxHealth);
            }
            else
            {
                currentHealth = Mathf.Max(0, savedHealth);
            }
        }

        if (healthText != null)
        {
            if (maxHealth > 0)
            {
                healthText.text = $"{currentHealth}/{maxHealth}";
            }
            else
            {
                healthText.text = currentHealth.ToString();
            }
        }
    }

    private void RefreshGold()
    {
        if (goldText != null)
        {
            goldText.text = RunSaveSystem.LoadGold().ToString();
        }
    }

    private void OnClickOpenSettings()
    {
        if (settingsPanelRoot == null)
        {
            return;
        }

        settingsPanelRoot.SetActive(true);
        PushBlockerIfNeeded();
        CloseConfirmReturnToBase();
    }

    private void CloseSettings(bool closeConfirmToo)
    {
        if (closeConfirmToo)
        {
            CloseConfirmReturnToBase();
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        PopBlockerIfNeeded();
    }

    private void OnClickMainMenu()
    {
        PopBlockerIfNeeded();
        CloseConfirmReturnToBase();
        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        RunAbortUtility.TryQuitRunAndLoadScene(mainMenuSceneName);
    }

    private void OnClickOpenConfirmReturnBase()
    {
        if (returnToBaseConfirmRoot != null)
        {
            returnToBaseConfirmRoot.SetActive(true);
        }
    }

    private void CloseConfirmReturnToBase()
    {
        if (returnToBaseConfirmRoot != null)
        {
            returnToBaseConfirmRoot.SetActive(false);
        }
    }

    private void OnClickConfirmYesReturnBase()
    {
        PopBlockerIfNeeded();
        CloseConfirmReturnToBase();
        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        RunAbortUtility.TryQuitRunAndLoadScene(baseSceneName);
    }

    private void PushBlockerIfNeeded()
    {
        if (!_blockingInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PushUIBlocker();
            _blockingInteractions = true;
        }
        else if (Interactions.Instance == null && errorTextOutput != null)
        {
            errorTextOutput.text = "Interactions missing on Combat scene.";
        }
    }

    private void PopBlockerIfNeeded()
    {
        if (_blockingInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PopUIBlocker();
            _blockingInteractions = false;
        }
    }
}
