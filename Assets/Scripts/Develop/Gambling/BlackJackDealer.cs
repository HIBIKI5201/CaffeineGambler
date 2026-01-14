using UnityEngine;
using Develop.Gambling.States;
using System.Collections.Generic;

namespace Develop.Gambling
{
    /// <summary>
    ///     ブラックジャックの進行役（ディーラー）。
    ///     データ保持と外部インターフェースを担当し、状態遷移は StateMachine に委譲する。
    /// </summary>
    public class BlackJackDealer : MonoBehaviour
    {
        /// <summary>
        ///     ブラックジャックの計算ロジック。
        /// </summary>
        public BlackJackLogic Logic => _logic;

        /// <summary>
        ///     ギャンブルの経済システム。
        /// </summary>
        public GamblingEconomy Economy => _economy;

        /// <summary>
        ///     現在の賭け金。
        /// </summary>
        public int CurrentBetAmount { get; set; }

        /// <summary>
        ///     依存関係を注入する初期化メソッド。
        /// </summary>
        /// <param name="logic">計算ロジック</param>
        /// <param name="economy">経済システム</param>
        public void Initialize(BlackJackLogic logic, GamblingEconomy economy)
        {
            // ロジックと経済システムを保持するため
            _logic = logic;
            _economy = economy;
            
            // 状態管理を開始するため
            _stateMachine = new BlackJackStateMachine();
            _stateMachine.Initialize(new IdleState(this, _stateMachine));
        }

        /// <summary>
        ///     ゲームを開始し、賭け金を支払う。
        /// </summary>
        /// <param name="betAmount">賭ける金額</param>
        public void StartGame(int betAmount)
        {
            // 現在のステートにベットイベントを通知するため
            _stateMachine.CurrentState?.OnBet(betAmount);
        }

        /// <summary>
        ///     プレイヤーがカードを引く。
        /// </summary>
        public void Hit()
        {
            // 現在のステートにヒット操作を委譲するため
            _stateMachine.CurrentState?.OnHit();
        }

        /// <summary>
        ///     プレイヤーがカードを引くのをやめ、勝負する。
        /// </summary>
        public void Stand()
        {
            // 現在のステートにスタンド操作を委譲するため
            _stateMachine.CurrentState?.OnStand();
        }

        /// <summary>
        ///     賭け金をリセットする。
        /// </summary>
        public void ResetBet()
        {
            // 次のゲームのためにベット額を初期化するため
            CurrentBetAmount = 0;
        }

        /// <summary>
        ///     現在の手札状況をログ出力（デバッグ用）
        /// </summary>
        public void LogHandStatus()
        {
            // プレイヤーとディーラーの現在の状況を可視化するため
            string pHand = string.Join(",", _logic.PlayerHand);
            int pScore = _logic.CalculateScore(new List<int>(_logic.PlayerHand));
            
            string dHand = string.Join(",", _logic.DealerHand);
            int dScore = _logic.CalculateScore(new List<int>(_logic.DealerHand));

            Debug.Log($"Player: [{pHand}] ({pScore}) vs Dealer: [{dHand}] ({dScore})");
        }

        private BlackJackLogic _logic;
        private GamblingEconomy _economy;
        private BlackJackStateMachine _stateMachine;
    }
}