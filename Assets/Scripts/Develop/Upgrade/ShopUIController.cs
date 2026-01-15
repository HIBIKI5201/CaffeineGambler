using UnityEngine;
using DG.Tweening;

namespace Develop.Upgrade
{
    /// <summary>
    ///     ショップのUIコントローラークラス。
    /// </summary>
    public class ShopUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform _shopPanel;
        [SerializeField] private float _animationDuration;
        [SerializeField] private Ease _animationEase;

        private float _width;
        private bool _isOpen = false;
        private Tween _currentTween;

        /// <summary> ショップの開閉をする。 </summary>
        private void ToggleShopPanel()
        {
            _currentTween?.Kill();
            if (_isOpen)
            {
                _currentTween = _shopPanel.DOAnchorPosX(-_width, _animationDuration).SetEase(_animationEase);
            }
            else
            {
                _currentTween = _shopPanel.DOAnchorPosX(0, _animationDuration).SetEase(_animationEase);
            }
            _isOpen = !_isOpen;
        }

        private void Start()
        {
            _width = _shopPanel.rect.width;

            _shopPanel.anchoredPosition = new Vector2(-_width, 0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleShopPanel();
            }
        }
    }
}
