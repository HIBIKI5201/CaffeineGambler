using Develop.Gambling;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Develop.Gambling.Develop
{
    /// <summary>
    ///     UIの表示更新と、ユーザー入力をDealerへ伝えるプレゼンタークラス。
    /// </summary>
    public class UiPresenter : MonoBehaviour
    {
        /// <summary>
        ///     Dealerの参照をセットする。
        /// </summary>
        /// <param name="dealer">操作対象のディーラー</param>
        public void SetDealer(BlackJackDealer dealer)
        {
            // UIからの操作をゲームロジックに伝えるための参照を保持するため
            _dealer = dealer;
        }

        /// <summary>
        ///     所持金表示を更新し、ベット可能額を制限する。
        /// </summary>
        /// <param name="money">最新の所持金</param>
        public void UpdateMoneyDisplay(int money)
        {
            // 現在の所持金を計算に利用するために保持するため
            _currentMoney = money;

            // 画面上の所持金テキストを更新するため
            if (_moneyText != null)
            {
                _moneyText.text = $"Money: {money}";
            }

            // 所持金が減って現在のベット額を下回った場合に、ベット額を所持金の上限に合わせるため
            if (_currentBetSelection > _currentMoney)
            {
                _currentBetSelection = _currentMoney;
                UpdateBetText();
            }
        }

        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _betAmountText;

        [Header("Bet Controls")]
        [SerializeField] private Button _increaseBetButton;
        [SerializeField] private Button _decreaseBetButton;

        [Header("Game Controls")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _hitButton;
        [SerializeField] private Button _standButton;

        private BlackJackDealer _dealer;
        private int _currentBetSelection = 100;
        private int _currentMoney;
        private const int BetStep = 100;

        private void Start()
        {
            // ベット額の増減ボタンにイベントを登録するため
            if (_increaseBetButton != null) _increaseBetButton.onClick.AddListener(OnIncreaseBet);
            if (_decreaseBetButton != null) _decreaseBetButton.onClick.AddListener(OnDecreaseBet);

            // ゲーム進行ボタンにイベントを登録するため
            if (_startButton != null) _startButton.onClick.AddListener(OnStartButtonClicked);
            if (_hitButton != null) _hitButton.onClick.AddListener(OnHitButtonClicked);
            if (_standButton != null) _standButton.onClick.AddListener(OnStandButtonClicked);

            // 初期状態の表示を更新するため
            UpdateBetText();
        }

        /// <summary>
        ///     ベット額を増やす処理。
        /// </summary>
        private void OnIncreaseBet()
        {
            // 所持金の範囲内でベット額を加算し、足りない場合は所持金全額をセットするため
            if (_currentBetSelection + BetStep <= _currentMoney)
            {
                _currentBetSelection += BetStep;
            }
            else
            {
                _currentBetSelection = _currentMoney;
            }
            UpdateBetText();
        }

        /// <summary>
        ///     ベット額を減らす処理。
        /// </summary>
        private void OnDecreaseBet()
        {
            // 0未満にならない範囲でベット額を減算するため
            if (_currentBetSelection - BetStep >= 0)
            {
                _currentBetSelection -= BetStep;
            }
            else
            {
                _currentBetSelection = 0;
            }
            UpdateBetText();
        }

        /// <summary>
        ///     ベット額のテキスト表示を更新する。
        /// </summary>
        private void UpdateBetText()
        {
            // 現在選択されているベット額を画面に反映するため
            if (_betAmountText != null)
            {
                _betAmountText.text = $"{_currentBetSelection}";
            }
        }

        /// <summary>
        ///     スタートボタン押下時の処理。
        /// </summary>
        private void OnStartButtonClicked()
        {
            // 0より大きい金額が賭けられている場合のみゲームを開始するため
            if (_currentBetSelection > 0)
            {
                _dealer?.StartGame(_currentBetSelection);
            }
        }

        /// <summary>
        ///     ヒットボタン押下時の処理。
        /// </summary>
        private void OnHitButtonClicked()
        {
            // プレイヤーのHit操作をDealerに伝えるため
            _dealer?.Hit();
        }

        /// <summary>
        ///     スタンドボタン押下時の処理。
        /// </summary>
        private void OnStandButtonClicked()
        {
            // プレイヤーのStand操作をDealerに伝えるため
            _dealer?.Stand();
        }
    }
}
