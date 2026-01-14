using UnityEngine;

namespace Develop.Gambling.States
{
    /// <summary>
    ///     カードを配布する状態。
    ///     初期配布を行い、自動的にプレイヤーのターンへ遷移する。
    /// </summary>
    public class DealState : BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">ディーラーの参照</param>
        /// <param name="stateMachine">ステートマシンの参照</param>
        public DealState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        /// <summary>
        ///     状態開始時の処理。
        /// </summary>
        public override void Enter()
        {
            // 処理の開始をログで明示するため
            Debug.Log("[State] Deal: カード配布中...");

            // 前回のゲームデータを破棄するため
            Dealer.Logic.ResetGame();

            // ルールに基づいた初期カードを配るため
            Dealer.Logic.DealInitialCards();

            // 配布後の状況を確認するため
            Dealer.LogHandStatus();

            // 配布完了後、即座にプレイヤーの選択フェーズへ移るため
            ToPlayerTurn();
        }

        /// <summary>
        ///     プレイヤーのターンへ遷移する。
        /// </summary>
        private void ToPlayerTurn()
        {
            // 状態を PlayerTurnState に切り替えるため
            StateMachine.ChangeState(new PlayerTurnState(Dealer, StateMachine));
        }
    }
}
