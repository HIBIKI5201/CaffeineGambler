using Develop.Gambling;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Develop.Gambling.Develop
{
    public class UiPresenter : MonoBehaviour
    {
        /// <summary>
        /// Dealerの参照をセットする。
        /// </summary>
        public void SetDealer(BlackJackDealer dealer)
        {
            _dealer = dealer;
        }

        /// <summary>
        /// 所持金表示を更新し、ベット可能額を制限する。
        /// </summary>
        public void UpdateMoneyDisplay(int money)
        {
            if (_moneyText != null)
            {
                _moneyText.text = $"Money: {money}";
            }
            UpdateBetLimit(money);
        }

        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _betAmountText;
        [SerializeField] private Slider _betSlider;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _hitButton;
        [SerializeField] private Button _standButton;

        private BlackJackDealer _dealer;
        private int _currentBetSelection;

        private void Start()
        {
            if (_betSlider != null)
            {
                _betSlider.onValueChanged.AddListener(OnBetSliderChanged);
                // 初期値反映
                OnBetSliderChanged(_betSlider.value);
            }

            // コードからイベントを登録することで、Inspectorでの設定漏れを防ぐ
            if (_startButton != null) _startButton.onClick.AddListener(OnStartButtonClicked);
            if (_hitButton != null) _hitButton.onClick.AddListener(OnHitButtonClicked);
            if (_standButton != null) _standButton.onClick.AddListener(OnStandButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            // ユーザーが選択したベット額でゲームを開始するため
            _dealer?.StartGame(_currentBetSelection);
        }

        private void OnHitButtonClicked()
        {
            // プレイヤーのHit操作をDealerへ伝えるため
            _dealer?.Hit();
        }

        private void OnStandButtonClicked()
        {
            // プレイヤーのStand操作をDealerへ伝えるため
            _dealer?.Stand();
        }

        private void OnBetSliderChanged(float value)
        {
            _currentBetSelection = Mathf.RoundToInt(value);

            if (_betAmountText != null)
            {
                _betAmountText.text = $"Bet: {_currentBetSelection}";
            }
        }

        private void UpdateBetLimit(int playerMoney)
        {
            if (_betSlider == null) return;

            // 所持金以上のベットを防ぐため最大値を更新
            _betSlider.maxValue = playerMoney;
            
            // 最低ベット額（10）を下回らないようにするが、所持金がそれ以下の場合は所持金を上限とする
            _betSlider.minValue = Mathf.Min(10, playerMoney);
        }
    }
}