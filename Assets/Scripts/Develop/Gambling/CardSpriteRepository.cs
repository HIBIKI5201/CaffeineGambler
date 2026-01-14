using System.Collections.Generic;
using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// カードのスプライトを一元管理するための ScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "CardSpriteRepository", menuName = "Gambling/Card Sprite Repository")]
    public class CardSpriteRepository : ScriptableObject
    {
        [System.Serializable]
        public struct CardSpriteMapping
        {
            public Suit Suit;
            public Rank Rank;
            public Sprite Sprite;
        }

        [Tooltip("カードのスート/ランクとスプライトの対応リスト")]
        [SerializeField]
        private List<CardSpriteMapping> _cardSprites;

        [Tooltip("カードの裏面の画像")]
        [SerializeField]
        private Sprite _cardBackSprite;
        public Sprite CardBackSprite => _cardBackSprite;

        private Dictionary<Card, Sprite> _spriteDictionary;

        /// <summary>
        /// ScriptableObjectがロードされた際に、検索を高速化するための辞書を作成する。
        /// </summary>
        private void OnEnable()
        {
            _spriteDictionary = new Dictionary<Card, Sprite>();
            if (_cardSprites == null) return;
            
            foreach (var mapping in _cardSprites)
            {
                var card = new Card(mapping.Suit, mapping.Rank);
                if (!_spriteDictionary.ContainsKey(card))
                {
                    _spriteDictionary.Add(card, mapping.Sprite);
                }
            }
        }

        /// <summary>
        /// 指定されたカードに対応するスプライトを取得する。
        /// </summary>
        public Sprite GetSprite(Card card)
        {
            if (_spriteDictionary == null || !_spriteDictionary.ContainsKey(card))
            {
                 // OnEnableが呼ばれていない場合や、辞書にない場合に備えて再構築を試みる
                OnEnable();
            }

            return _spriteDictionary.TryGetValue(card, out var sprite) ? sprite : null;
        }

#if UNITY_EDITOR
        // このフィールドはエディタ拡張でのみ使用する
        [Header("Editor Only")]
        [Tooltip("スプライトが格納されているプロジェクト内のフォルダパス (例: Assets/MyArt/Cards)")]
        [SerializeField] private string _spriteFolderPathForEditor;

        public List<CardSpriteMapping> CardSprites
        {
            get => _cardSprites;
            set => _cardSprites = value;
        }
        public string SpriteFolderPathForEditor => _spriteFolderPathForEditor;
#endif
    }
}
