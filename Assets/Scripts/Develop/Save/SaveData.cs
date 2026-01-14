using UnityEngine;

namespace Develop.Save
{
    /// <summary>
    ///     プレイヤーのデータを保存・読み込みするクラス。
    ///     所持金や強化レベルなどを保存・読み込みする。
    /// </summary>
    public static class SaveData
    {
         /// <summary> Int値を保存する</summary>
        public static void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        /// <summary> Int値を読み込む </summary>
        public static int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        /// <summary> 文字列を保存する </summary>
        public static void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        /// <summary> 文字列を読み込む </summary>
        public static string LoadString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
    }
}