using UnityEngine;
using Develop.Player;
using UniRx;

namespace Develop.Gambling.Develop
{
    /// <summary>
    ///     ギャンブルシステムの初期化を行うクラス。
    ///     ScriptableObject を含む依存関係の注入(DI)を担当する。
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
            // ゲームに必要な設定ファイルが存在するか確認するため
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

            // プレイヤーの所持金データを生成し、UIと同期させるため
            PlayerData playerData = new PlayerData(InitialPlayerMoney);
            playerData.Money.Subscribe(money =>
            {
                _uiPresenter.UpdateMoneyDisplay(money);
            })
            .AddTo(this);
                
            // 経済システムと計算ロジックをインスタンス化するため
            GamblingEconomy economy = new GamblingEconomy(playerData, _economySettings);
            BlackJackLogic logic = new BlackJackLogic(_blackJackSettings);

            _dealer = BlackJackDealerLocator.GetDealer();
            // 生成したロジック等をディーラーに注入して初期化するため
            _dealer.Initialize(logic, economy);

            // UIがディーラーを操作できるように参照を渡すため
            _uiPresenter.SetDealer(_dealer);

            Debug.Log("Gambling System Initialized with separate ScriptableObject settings.");
        }
    }
}