using DG.Tweening;
using UnityEngine;

public class MapNodeView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject availabilityIndicator;
    [SerializeField] private GameObject currentIndicator;
    [SerializeField] private GameObject visitedIndicator;
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private float hoverScaleMultiplier = 1.12f;
    [SerializeField] private float currentScaleMultiplier = 1.2f;
    [SerializeField] private float scaleTweenDuration = 0.15f;

    public MapNode MapNode { get; private set; }

    private bool isHovered;
    private Tweener scaleTween;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }

    public void Setup(MapNode mapNode)
    {
        MapNode = mapNode;

        if (spriteRenderer != null && mapNode.Blueprint != null)
        {
            spriteRenderer.sprite = mapNode.Blueprint.Image;
        }

        isHovered = false;
        SetAvailable(false);
        SetCurrent(false);
        SetVisited(false);
    }

    public void SetAvailable(bool available)
    {
        if (MapNode != null)
        {
            MapNode.SetAvailable(available);
        }

        if (availabilityIndicator != null)
        {
            availabilityIndicator.SetActive(available);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = available ? availableColor : lockedColor;
        }

        if (!available)
        {
            isHovered = false;
        }

        RefreshScale();
    }

    public void SetCurrent(bool isCurrent)
    {
        if (MapNode != null)
        {
            MapNode.SetCurrent(isCurrent);
        }

        if (currentIndicator != null)
        {
            currentIndicator.SetActive(isCurrent);
        }

        RefreshScale();
    }

    public void SetVisited(bool visited)
    {
        if (MapNode != null)
        {
            MapNode.SetVisited(visited);
        }

        if (visitedIndicator != null)
        {
            visitedIndicator.SetActive(visited);
        }
    }

    private void OnMouseEnter()
    {
        if (MapNode == null || !MapNode.IsAvailable)
        {
            return;
        }

        isHovered = true;
        RefreshScale();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        RefreshScale();
    }

    private void RefreshScale()
    {
        if (MapNode == null)
        {
            return;
        }

        float multiplier = 1f;

        if (MapNode.IsCurrent)
        {
            multiplier = currentScaleMultiplier;
        }
        else if (isHovered && MapNode.IsAvailable)
        {
            multiplier = hoverScaleMultiplier;
        }

        scaleTween?.Kill();
        scaleTween = transform.DOScale(Vector3.one * multiplier, scaleTweenDuration).SetEase(Ease.OutQuad);
    }
}

