using UnityEngine;

public static class SaveSlotSystem
{
    public const int SlotCount = 3;

    private const string ActiveSlotKey = "ActiveSaveSlot";
    private const string SlotExistsKeyPrefix = "SaveSlotExists_";

    public static int GetActiveSlot()
    {
        int slot = PlayerPrefs.GetInt(ActiveSlotKey, 0);
        return Mathf.Clamp(slot, 0, SlotCount - 1);
    }

    public static void SetActiveSlot(int slotIndex)
    {
        int clamped = Mathf.Clamp(slotIndex, 0, SlotCount - 1);
        PlayerPrefs.SetInt(ActiveSlotKey, clamped);
        PlayerPrefs.Save();
    }

    public static bool GetSlotExists(int slotIndex)
    {
        return PlayerPrefs.GetInt(GetSlotExistsKey(slotIndex), 0) == 1;
    }

    public static void SetSlotExists(int slotIndex, bool exists)
    {
        PlayerPrefs.SetInt(GetSlotExistsKey(slotIndex), exists ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool HasAnyStartedSlot()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (GetSlotExists(i))
            {
                return true;
            }
        }

        return false;
    }

    public static string MakeSlotKey(string baseKey)
    {
        return $"slot_{GetActiveSlot()}_{baseKey}";
    }

    private static string GetSlotExistsKey(int slotIndex)
    {
        return SlotExistsKeyPrefix + Mathf.Clamp(slotIndex, 0, SlotCount - 1);
    }
}
