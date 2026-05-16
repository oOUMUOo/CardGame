using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSystem : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string baseSceneName = "BaseScene";

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject slotsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Slots")]
    [SerializeField] private SaveSlotButtonUI[] slotButtons;

    private SaveSlotMode currentSlotMode = SaveSlotMode.NewGame;

    private void Start()
    {
        ShowMainPanel();
        RefreshMainButtons();
    }

    public void OnClickNewGame()
    {
        OpenSlots(SaveSlotMode.NewGame);
    }

    public void OnClickContinue()
    {
        OpenSlots(SaveSlotMode.Continue);
    }

    public void OnClickSettings()
    {
        SetPanelState(false, false, true);
    }

    public void OnClickBackToMain()
    {
        ShowMainPanel();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SaveSlotSystem.SlotCount)
        {
            return;
        }

        bool slotExists = SaveSlotSystem.GetSlotExists(slotIndex);
        if (currentSlotMode == SaveSlotMode.Continue && !slotExists)
        {
            return;
        }

        SaveSlotSystem.SetActiveSlot(slotIndex);

        if (currentSlotMode == SaveSlotMode.NewGame)
        {
            MapSaveSystem.ClearSave();
            RunSaveSystem.ClearHeroHealth();
            RunSaveSystem.ClearGold();
            RunRareOffsetSystem.Clear();
            RunDeckBonusCardsSystem.Clear();
            DeckPresetSaveSystem.ClearForActiveSlot();
            SaveSlotSystem.SetSlotExists(slotIndex, true);
        }

        SceneManager.LoadScene(baseSceneName);
    }

    private void ShowMainPanel()
    {
        SetPanelState(true, false, false);
        RefreshMainButtons();
    }

    private void OpenSlots(SaveSlotMode mode)
    {
        currentSlotMode = mode;
        SetPanelState(false, true, false);
        RefreshSlots();
    }

    private void SetPanelState(bool showMain, bool showSlots, bool showSettings)
    {
        if (mainPanel != null) mainPanel.SetActive(showMain);
        if (slotsPanel != null) slotsPanel.SetActive(showSlots);
        if (settingsPanel != null) settingsPanel.SetActive(showSettings);
    }

    private void RefreshMainButtons()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(SaveSlotSystem.HasAnyStartedSlot());
        }
    }

    private void RefreshSlots()
    {
        if (slotButtons == null)
        {
            return;
        }

        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] == null)
            {
                continue;
            }

            bool occupied = SaveSlotSystem.GetSlotExists(i);
            slotButtons[i].Setup(i, occupied, currentSlotMode, OnClickSlot);
        }
    }
}

public enum SaveSlotMode
{
    NewGame,
    Continue
}
