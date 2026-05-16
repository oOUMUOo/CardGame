using UnityEngine;

public static class RunRareOffsetSystem
{
    private const string KeyBase = "RunRareCardOffsetPercent";
    public const int MinOffset = -5;
    public const int MaxOffset = 40;

    private static string Key => SaveSlotSystem.MakeSlotKey(KeyBase);

    public static int GetOffset()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(Key, MinOffset), MinOffset, MaxOffset);
    }

    public static void SetOffset(int value)
    {
        PlayerPrefs.SetInt(Key, Mathf.Clamp(value, MinOffset, MaxOffset));
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        if (PlayerPrefs.HasKey(Key))
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
