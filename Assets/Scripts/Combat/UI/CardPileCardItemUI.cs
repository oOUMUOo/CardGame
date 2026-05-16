using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPileCardItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;

    public void Setup(CardData cardData)
    {
        if (cardData == null)
        {
            return;
        }

        Setup(new Card(cardData));
    }

    public void Setup(Card card)
    {
        if (card == null)
        {
            return;
        }

        if (titleText != null)
        {
            titleText.text = card.Title;
        }

        if (manaText != null)
        {
            manaText.text = card.Mana.ToString();
        }

        if (descriptionText != null)
        {
            descriptionText.text = card.Description;
        }

        if (artworkImage != null)
        {
            artworkImage.sprite = card.Image;
        }
    }

    public void SetInteractable(bool interactable)
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = interactable;
        }
    }

    public void SetClick(UnityAction onClick)
    {
        var btn = GetComponent<Button>();
        if (btn == null)
        {
            btn = gameObject.AddComponent<Button>();
            btn.targetGraphic = GetComponentInChildren<Image>(true);
            btn.transition = Selectable.Transition.None;
        }

        btn.onClick.RemoveAllListeners();
        if (onClick != null)
        {
            btn.onClick.AddListener(onClick);
        }
    }
}
