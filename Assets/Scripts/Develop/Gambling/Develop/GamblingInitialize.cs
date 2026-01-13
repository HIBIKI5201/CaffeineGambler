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
        [Header("Settings")]
        [SerializeField] private BlackJackSettings _blackJackSettings;
        [SerializeField] private GamblingEconomySettings _economySettings;
        [SerializeField] private UiPresenter _uiPresenter;

        private const int InitialPlayerMoney = 1000;
        private BlackJackDealer _dealer;

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
            
            // ロケーターを使用してディーラーとその依存関係を一括で解決（生成・初期化）するため
            _dealer = BlackJackDealerLocator.Resolve(_blackJackSettings, _economySettings, playerData);

            // UIがディーラーを操作できるように参照を渡すため
            _uiPresenter.SetDealer(_dealer);

            Debug.Log("Gambling System Initialized using Locator.");
        }

        private void OnDestroy()
        {
            // シーン遷移や終了時に参照をクリアするため
            BlackJackDealerLocator.Clear();
        }
    }
}