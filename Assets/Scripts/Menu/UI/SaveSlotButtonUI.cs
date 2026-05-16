using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private Button button;

    private int slotIndex;
    private Action<int> clickAction;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(int index, bool occupied, SaveSlotMode mode, Action<int> onClick)
    {
        slotIndex = index;
        clickAction = onClick;

        if (titleText != null)
        {
            titleText.text = $"Слот {index + 1}";
        }

        bool interactable = mode == SaveSlotMode.NewGame || occupied;
        if (button != null)
        {
            button.interactable = interactable;
        }

        if (stateText != null)
        {
            if (mode == SaveSlotMode.NewGame)
            {
                stateText.text = occupied ? "Перезаписать" : "Пусто";
            }
            else
            {
                stateText.text = occupied ? "Продолжить" : "Нет сохранения";
            }
        }
    }

    private void HandleClick()
    {
        clickAction?.Invoke(slotIndex);
    }
}
