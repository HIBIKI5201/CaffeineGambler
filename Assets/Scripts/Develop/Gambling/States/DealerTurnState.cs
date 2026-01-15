using UnityEngine;

namespace Develop.Gambling.States
{
    /// <summary>
    ///     ディーラーの行動状態。
    ///     規定のルールに従って自動的にカードを引き、結果フェーズへ遷移する。
    /// </summary>
    public class DealerTurnState : BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">ディーラーの参照</param>
        /// <param name="stateMachine">ステートマシンの参照</param>
        public DealerTurnState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        /// <summary>
        ///     状態開始時の処理。
        /// </summary>
        public override void Enter()
        {
            // ディーラーの処理フェーズであることをログで示すため
            Debug.Log("[State] DealerTurn: ディーラーのドロー処理");

            // ロジックに基づいてディーラーの手札を確定させるため
            Dealer.Logic.DealerTurn();

            // 全てのカードが揃ったため、勝敗判定フェーズへ移行するため
            StateMachine.ChangeState(new ResultState(Dealer, StateMachine));
        }
    }
}
