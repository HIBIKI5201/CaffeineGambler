using Develop.Gambling;
using Develop.Gambling.Develop;
using Develop.Player;
using UnityEngine;
using UniRx;
namespace Runtime.Gambling
{
    /// <summary>
    /// ギャンブルシステムの初期化を行うクラス。
    /// </summary>
    public class GamblingInitilizer : MonoBehaviour
    {
        /// <summary>
        /// ギャンブルシステム全体の初期化を行う。 
        /// </summary>
        public void GamblingInit(PlayerData playerData)
        {
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
        

        [Header("Settings")]
        [SerializeField] private BlackJackSettings _blackJackSettings;
        [SerializeField] private GamblingEconomySettings _economySettings;

        [Header("UI & Presentation")]
        [SerializeField] private UiPresenter _uiPresenter;
        [SerializeField] private DealerPresenter _dealerPresenter;


    }
}
