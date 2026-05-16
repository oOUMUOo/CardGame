using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPileButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text pileNameText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button button;

    private Action clickAction;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
            button.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(string pileName, Action onClick)
    {
        clickAction = onClick;

        if (pileNameText != null)
        {
            pileNameText.text = pileName;
        }
    }

    public void SetCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }

    private void HandleClick()
    {
        clickAction?.Invoke();
    }
}
