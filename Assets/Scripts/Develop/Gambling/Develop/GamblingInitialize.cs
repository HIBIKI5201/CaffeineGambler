using Develop.Player;
using UniRx;
using UnityEngine;
namespace Develop.Gambling.Develop
{
    /// <summary>
    /// ギャンブルシステムの初期化を行うクラス。
    /// 依存性の注入(DI)を担当する。
    /// </summary>
    public class GamblingInitialize : MonoBehaviour
    {
        // 実際にはGameManagerやInfrastracture層から取得する想定
        // ここではテスト用にInspectorで設定、あるいは内部生成する
        [SerializeField] private BlackJackDealer _dealer;
        [SerializeField] private UiPresenter _uiPresenter;
        private PlayerData _playerData;
        private void Start()
        {
           
            _playerData = new PlayerData(1000);
            _playerData.Money.Subscribe(money =>
            {
                _uiPresenter.UpdateMoneyDisplay(money);
            });

            // 2. 各層の生成とDI
            // 経済層
            GamblingEconomy economy = new GamblingEconomy(_playerData);
            
            // 勝負層
            BlackJackLogic logic = new BlackJackLogic();

            _dealer.Initialize(logic, economy);

            Debug.Log("Gambling System Initialized.");

          
        }
    }
}
