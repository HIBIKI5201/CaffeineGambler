using UnityEngine;
using UnityEngine.UI;

namespace Develop.Gambling
{
    /// <summary>
    /// カードのGameObjectにアタッチし、見た目や挙動を制御するクラス。
    /// スプライトの表示や、カード情報の保持を担当する。
    /// </summary>
  //  [RequireComponent(typeof(SpriteRenderer))]
    public class CardView : MonoBehaviour
    {
        public Card CardData { get; private set; }
        public bool IsFaceUp { get; private set; }

        private Image _image;
        private Sprite _frontSprite;
        private Sprite _backSprite;

        private void Awake()
        {
           _image = GetComponent<Image>();
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
            _image.sprite = _backSprite;
        }

        /// <summary>
        /// カードを裏返す。
        /// </summary>
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
            _image.sprite = IsFaceUp ? _frontSprite : _backSprite;
        }

      
    }
}
