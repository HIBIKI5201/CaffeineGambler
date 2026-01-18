using UnityEngine;
using UnityEditor;

public static class PlayerPrefsResetMenu
{
    [MenuItem("Tools/PlayerPrefs/ResetAll")]
    public static void ResetAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefsをすべてリセット");
    }
}
