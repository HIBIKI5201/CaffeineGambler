using System;
using UnityEngine;

namespace Develop.Save
{
    /// <summary>
    ///     プレイヤーのデータを保存・読み込みするクラス。
    ///     所持金や強化レベルなどを保存・読み込みする。
    /// </summary>
    public static class SaveData
    {
        /// <summary> 整数を保存する</summary>
        public static void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        /// <summary> 整数を読み込む </summary>
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

        /// <summary> オブジェクトをJSON形式で保存する </summary>
        public static void SaveJson(Type keyType, object data)
        {
            // クラス名をキーとして使用
            string key = keyType.Name;
            string json = JsonUtility.ToJson(data);
            SaveString(key, json);
        }

        /// <summary> オブジェクトをJSON形式で読み込む </summary>
        public static T LoadJson<T>(Type keyType, T defaultValue = default)
        {
            // クラス名をキーとして使用
            string key = keyType.Name;
            string json = LoadString(key);
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue;
            }
            return JsonUtility.FromJson<T>(json);
        }
    }
}