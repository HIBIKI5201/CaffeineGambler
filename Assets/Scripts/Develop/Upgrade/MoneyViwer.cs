using Develop.Save;
using TMPro;
using UniRx;
using UnityEngine;
using Develop.Player;
namespace Develop.Upgrade
{
    /// <summary>
    ///     お金表示クラス。
    /// </summary>
    public class MoneyViwer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyText;

        private readonly CompositeDisposable _disposables = new();

        public void Bind(PlayerData playerData)
        {
            _disposables.Clear();

            playerData.Money
                .Subscribe(m => _moneyText.text = m.ToString())
                .AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
