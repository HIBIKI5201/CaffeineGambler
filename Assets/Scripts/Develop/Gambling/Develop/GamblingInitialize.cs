using UnityEngine;
using Develop.Player;
using UniRx;

namespace Develop.Gambling.Develop
{
    /// <summary>
    /// ギャンブルシステムの初期化を行うクラス。
    /// Pureなロジッククラスのインスタンス化と、MonoBehaviourへの依存注入(DI)を一括で担当する。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GamblingInitialize : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private BlackJackSettings _blackJackSettings;
        [SerializeField] private GamblingEconomySettings _economySettings;

        [Header("UI & Presentation")]
        [SerializeField] private UiPresenter _uiPresenter;
        [SerializeField] private DealerPresenter _dealerPresenter;

        private const int InitialPlayerMoney = 1000;
        private BlackJackDealer _dealer;

        /// <summary>
        /// UnityのAwakeライフサイクルでシステム全体の構築を行う。
        /// </summary>
        private void Awake()
        {
            // 必要な設定ファイルとプレゼンターの存在チェック。不備があれば早期リターン
            if (!ValidateDependencies()) return;

            // 1. データの生成（Pure Data）
            var playerData = new PlayerData(InitialPlayerMoney);
            
            // UIとの同期設定
            playerData.Money
                .Subscribe(money => _uiPresenter.UpdateMoneyDisplay(money))
                .AddTo(this);
            
            // 2. Pureな計算・判定ロジックの生成（Pure Logic）
            var scoreCalculator = new BlackJackScoreCalculator(_blackJackSettings);
            var resultDeterminer = new BlackJackResultDeterminer(scoreCalculator);
            
            // 3. 状態管理・進行ロジックの生成
            var bjLogic = new BlackJackLogic(_blackJackSettings, scoreCalculator, resultDeterminer);
            var economy = new GamblingEconomy(playerData, _economySettings);

            var dealer = new BlackJackDealer(
                bjLogic,
                economy,
                _dealerPresenter
            );


            // 5. 各プレゼンターに操作対象のディーラーをセット
            _uiPresenter.SetDealer(dealer);
            _dealerPresenter.SetDealer(dealer);

            Debug.Log("Gambling System Initialized via manual Dependency Injection.");
        }

        /// <summary>
        /// インスペクターで設定された依存関係が有効か検証する。
        /// </summary>
        /// <returns>すべて有効なら true</returns>
        private bool ValidateDependencies()
        {
            if (_blackJackSettings == null)
            {
                Debug.LogError("BlackJackSettings が未設定です。");
                return false;
            }
            if (_economySettings == null)
            {
                Debug.LogError("GamblingEconomySettings が未設定です。");
                return false;
            }
            if (_uiPresenter == null)
            {
                Debug.LogError("UiPresenter が未設定です。");
                return false;
            }
            if (_dealerPresenter == null)
            {
                Debug.LogError("DealerPresenter が未設定です。");
                return false;
            }
            return true;
        }
    }
}