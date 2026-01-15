using UnityEngine;

namespace Develop.Upgrade
{
    /// <summary>
    /// ショップアイテムのビュー。
    /// </summary>
    public class ShopItemView : MonoBehaviour
    {
        public UnityEngine.UI.Button BuyButton => _buyButton;

        [SerializeField] private TMPro.TextMeshProUGUI _nameText;
        [SerializeField] private TMPro.TextMeshProUGUI _levelText;
        [SerializeField] private TMPro.TextMeshProUGUI _costText;
        [SerializeField] private UnityEngine.UI.Button _buyButton;

        public void Set(IUpgrade upgrade)
        {
            _nameText.text = upgrade.Name;
            _levelText.text = $"Lv.{upgrade.Level}/{upgrade.MaxLevel}";
            _costText.text = $"Price{upgrade.GetCost()}";
        }
    }
}
