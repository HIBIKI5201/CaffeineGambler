using UnityEngine;
using Develop.Player;
using UniRx;
namespace Develop.Gambling.Develop
{
    /// <summary>
    /// ギャンブルシステムの初期化を行うクラス。
    /// ScriptableObject を含む依存関係の注入(DI)を担当する。
    /// </summary>
    public class GamblingInitialize : MonoBehaviour
    {
        [SerializeField] private BlackJackDealer _dealer;
        [Header("Settings")]
        [SerializeField] private BlackJackSettings _blackJackSettings;
        [SerializeField] private GamblingEconomySettings _economySettings;
        [SerializeField] private UiPresenter _uiPresenter;

        private const int InitialPlayerMoney = 1000;

        private void Start()
        {
            // 1. 設定のバリデーション
            if (_blackJackSettings == null)
            {
                Debug.LogError("BlackJackSettings が設定されていません。初期化を中止します。");
                return;
            }
            if (_economySettings == null)
            {
                Debug.LogError("GamblingEconomySettings が設定されていません。初期化を中止します。");
                return;
            }

            // 2. 共有データの取得（シミュレート）
            PlayerData playerData = new PlayerData(InitialPlayerMoney);
            playerData.Money.Subscribe(money =>
            {
                _uiPresenter.UpdateMoneyDisplay(money);
            })
                .AddTo(this);
                

            // 3. 各層の生成とDI
            // 経済層（EconomySettingsを注入）
            GamblingEconomy economy = new GamblingEconomy(playerData, _economySettings);
            
            // 勝負層（BlackJackSettingsを注入）
            BlackJackLogic logic = new BlackJackLogic(_blackJackSettings);

            // 入力層へ注入
            if (_dealer == null)
            {
                GameObject go = new GameObject("BlackJackDealer");
                _dealer = go.AddComponent<BlackJackDealer>();
            }

            _dealer.Initialize(logic, economy);
            
            // UI層へDealerを注入
            _uiPresenter.SetDealer(_dealer);

            Debug.Log("Gambling System Initialized with separate ScriptableObject settings.");
        }
    }
}