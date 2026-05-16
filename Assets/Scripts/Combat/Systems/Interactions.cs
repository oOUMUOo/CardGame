using UnityEngine;

public class Interactions : Singleton<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;
    private int uiBlockersCount = 0;

    public bool PlayerCanInteract()
    {
        if (uiBlockersCount > 0) return false;
        return !ActionSystem.Instance.IsPerforming;
    }

    public bool PlayerCanHover()
    {
        if (PlayerIsDragging) return false;
        if (uiBlockersCount > 0) return false;
        return true;
    }

    public void PushUIBlocker()
    {
        uiBlockersCount++;
    }

    public void PopUIBlocker()
    {
        uiBlockersCount = Mathf.Max(0, uiBlockersCount - 1);
    }
}
