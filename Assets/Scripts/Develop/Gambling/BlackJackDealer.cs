using UnityEngine;
using Develop.Gambling.States;
using System.Collections.Generic;
using System.Linq;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの進行役（ディーラー）。
    /// データ保持と外部インターフェースを担当し、状態遷移は StateMachine に委譲する。
    /// </summary>
    public class BlackJackDealer : MonoBehaviour
    {
        // 外部からアクセス可能なプロパティ
        public BlackJackLogic Logic => _logic;
        public GamblingEconomy Economy => _economy;
        public int CurrentBetAmount { get; set; }

        /// <summary>
        /// 依存関係を注入する初期化メソッド。
        /// </summary>
        public void Initialize(BlackJackLogic logic, GamblingEconomy economy)
        {
            _logic = logic;
            _economy = economy;
            
            // ステートマシンの生成と初期化
            _stateMachine = new BlackJackStateMachine();
            _stateMachine.Initialize(new IdleState(this, _stateMachine));
        }

        /// <summary>
        /// ゲームを開始し、賭け金を支払う。
        /// </summary>
        public void StartGame(int betAmount)
        {
            _stateMachine.CurrentState?.OnBet(betAmount);
        }

        /// <summary>
        /// プレイヤーがカードを引く。
        /// </summary>
        public void Hit()
        {
            _stateMachine.CurrentState?.OnHit();
        }

        /// <summary>
        /// プレイヤーがカードを引くのをやめ、勝負する。
        /// </summary>
        public void Stand()
        {
            _stateMachine.CurrentState?.OnStand();
        }

        /// <summary>
        /// 賭け金をリセットする。
        /// </summary>
        public void ResetBet()
        {
            CurrentBetAmount = 0;
        }

        /// <summary>
        /// 現在の手札状況をログ出力（デバッグ用）
        /// </summary>
        public void LogHandStatus()
        {
            // Cardオブジェクトのリストを文字列に変換
            string pHand = string.Join(",", _logic.PlayerHand.Select(card => card.ToString()));
            int pScore = _logic.CalculateScore(_logic.PlayerHand);
            
            string dHand = string.Join(",", _logic.DealerHand.Select(card => card.ToString()));
            int dScore = _logic.CalculateScore(_logic.DealerHand);

            Debug.Log($"Player: [{pHand}] ({pScore}) vs Dealer: [{dHand}] ({dScore})");
        }

        private BlackJackLogic _logic;
        private GamblingEconomy _economy;
        private BlackJackStateMachine _stateMachine;
    }
}
