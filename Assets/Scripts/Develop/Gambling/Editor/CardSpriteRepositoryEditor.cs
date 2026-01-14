#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Develop.Gambling.Editor
{
    [CustomEditor(typeof(CardSpriteRepository))]
    public class CardSpriteRepositoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // デフォルトのインスペクターを表示
            base.OnInspectorGUI();

            // ターゲットオブジェクトを取得
            var repository = (CardSpriteRepository)target;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("下のボタンを押すと、指定されたフォルダパスからスプライトを自動で検索し、リストを上書きします。ファイル名からスートとランクを推測します（例: Spade_A, Club_10 など）。", MessageType.Info);

            // ボタンを描画
            if (GUILayout.Button("スプライトを自動読み込み"))
            {
                // 確認ダイアログを表示
                if (EditorUtility.DisplayDialog("スプライトの自動読み込み",
                    "指定されたフォルダパスからスプライトを検索し、リストを上書きします。よろしいですか？", "はい", "いいえ"))
                {
                    FindAndSetSprites(repository);
                }
            }
        }

        private void FindAndSetSprites(CardSpriteRepository repository)
        {
            string path = repository.SpriteFolderPathForEditor;
            if (string.IsNullOrEmpty(path) || !AssetDatabase.IsValidFolder(path))
            {
                Debug.LogError("指定されたフォルダパスが無効です。Assetsから始まる正しいパスを入力してください。 (例: Assets/Art/MyCards)");
                return;
            }

            // 指定されたパス内の全スプライトアセットのGUIDを検索
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"指定されたフォルダ '{path}' にスプライトが見つかりませんでした。");
                return;
            }

            var newMappings = new List<CardSpriteRepository.CardSpriteMapping>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                
                string fileName = Path.GetFileNameWithoutExtension(sprite.name);

                if (TryParseCard(fileName, out Suit suit, out Rank rank))
                {
                    newMappings.Add(new CardSpriteRepository.CardSpriteMapping
                    {
                        Suit = suit,
                        Rank = rank,
                        Sprite = sprite
                    });
                }
                else
                {
                    // card_back のようなファイルは警告せずスキップする
                    if (fileName.Equals("card_back", System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue; 
                    }
                    Debug.LogWarning($"ファイル名からカード情報を推測できませんでした: {fileName}");
                }
            }
            
            // リストを更新
            repository.CardSprites = newMappings;
            
            // 変更を保存
            EditorUtility.SetDirty(repository);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"{newMappings.Count}個のスプライトを読み込み、リストを更新しました。");
        }

        private bool TryParseCard(string fileName, out Suit suit, out Rank rank)
        {
            suit = default;
            rank = default;

            if (string.IsNullOrEmpty(fileName)) return false;

            string[] parts = fileName.Split('_');
            if (parts.Length != 2) return false; // "{rank}_{suit}" の形式ではない

            // ランクのパース
            if (int.TryParse(parts[0], out int rankInt))
            {
                switch (rankInt)
                {
                    case 1:  rank = Rank.Ace; break;
                    case 2:  rank = Rank.Two; break;
                    case 3:  rank = Rank.Three; break;
                    case 4:  rank = Rank.Four; break;
                    case 5:  rank = Rank.Five; break;
                    case 6:  rank = Rank.Six; break;
                    case 7:  rank = Rank.Seven; break;
                    case 8:  rank = Rank.Eight; break;
                    case 9:  rank = Rank.Nine; break;
                    case 10: rank = Rank.Ten; break;
                    case 11: rank = Rank.Jack; break;
                    case 12: rank = Rank.Queen; break;
                    case 13: rank = Rank.King; break;
                    default: return false; // 未知のランク
                }
            }
            else
            {
                return false; // ランクが数字ではない
            }

            // スートのパース
            string suitString = parts[1]; // 例: "club"

            // ファイル名から抽出したスート文字列 (例: "club") をSuit enumの複数形に変換
            switch (suitString.ToLower())
            {
                case "club":    suitString = "Clubs"; break;
                case "diamond": suitString = "Diamonds"; break;
                case "heart":   suitString = "Hearts"; break;
                case "spade":   suitString = "Spades"; break;
                default: return false; // 未知のスート名
            }
            
            if (System.Enum.TryParse(suitString, true, out suit)) // 大文字小文字を無視してパース
            {
                // スートとランクが両方見つかった
                return true;
            }

            return false;
        }
        
        // ファイル名に含まれるランクの文字列を返す (今回は数字)
        private static string ConvertRankToFileName(Rank rank)
        {
            switch (rank)
            {
                case Rank.Ace:   return "1";
                case Rank.Jack:  return "11";
                case Rank.Queen: return "12";
                case Rank.King:  return "13";
                default: return ((int)rank).ToString(); // 2-10はそのまま数字
            }
        }
    }
}
#endif