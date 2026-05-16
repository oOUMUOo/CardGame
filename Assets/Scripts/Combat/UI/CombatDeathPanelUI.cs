using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Панель поражения: показывается при смерти героя; кнопка завершает забег и грузит базу.
/// </summary>
public class CombatDeathPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanelRoot;
    [SerializeField] private Button continueToBaseButton;
    [SerializeField] private string baseSceneName = "BaseScene";

    private bool _blockingInteractions;

    private void Awake()
    {
        if (deathPanelRoot != null && deathPanelRoot.activeSelf)
        {
            deathPanelRoot.SetActive(false);
        }

        if (continueToBaseButton != null)
        {
            continueToBaseButton.onClick.AddListener(OnClickContinueToBase);
        }
    }

    private void OnDestroy()
    {
        if (continueToBaseButton != null)
        {
            continueToBaseButton.onClick.RemoveListener(OnClickContinueToBase);
        }

        if (_blockingInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PopUIBlocker();
            _blockingInteractions = false;
        }
    }

    public void Show()
    {
        if (deathPanelRoot != null)
        {
            deathPanelRoot.SetActive(true);
        }

        PushBlockerIfNeeded();
    }

    private void OnClickContinueToBase()
    {
        PopBlockerIfNeeded();
        RunAbortUtility.TryQuitRunAndLoadScene(baseSceneName);
    }

    private void PushBlockerIfNeeded()
    {
        if (!_blockingInteractions && Interactions.Instance != null)
        {
            Interactions.Instance.PushUIBlocker();
            _blockingInteractions = true;
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
