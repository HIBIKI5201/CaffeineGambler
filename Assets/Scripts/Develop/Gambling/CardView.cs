using UnityEngine;

namespace Develop.Gambling
{
    /// <summary>
    /// カードのGameObjectにアタッチし、見た目や挙動を制御するクラス。
    /// スプライトの表示や、カード情報の保持を担当する。
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CardView : MonoBehaviour
    {
        public Card CardData { get; private set; }
        public bool IsFaceUp { get; private set; }
        /// <summary>
        /// 現在の描画順を取得する
        /// </summary>
        public int SortingOrder
        {
            get
            {
                return _spriteRenderer.sortingOrder;
            }
        }
        private SpriteRenderer _spriteRenderer;
        private Sprite _frontSprite;
        private Sprite _backSprite;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// カードの情報を初期化し、裏面で表示する。
        /// </summary>
        /// <param name="card">カードのデータ</param>
        /// <param name="frontSprite">カードの表面のスプライト</param>
        /// <param name="backSprite">カードの裏面のスプrite</param>
        public void Initialize(Card card, Sprite frontSprite, Sprite backSprite)
        {
            CardData = card;
            _frontSprite = frontSprite;
            _backSprite = backSprite;

            IsFaceUp = false;
            _spriteRenderer.sprite = _backSprite;
        }

        /// <summary>
        /// カードを裏返す。
        /// </summary>
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
            _spriteRenderer.sprite = IsFaceUp ? _frontSprite : _backSprite;
        }

        /// <summary>
        /// スプライトの描画順を設定する。値が大きいほど手前に描画される。
        /// </summary>
        /// <param name="order">描画順の値</param>
        public void SetSortingOrder(int order)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = order;
            }
        }
    }
}
