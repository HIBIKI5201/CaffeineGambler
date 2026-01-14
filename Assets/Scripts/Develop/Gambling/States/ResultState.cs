using UnityEngine;

namespace Develop.Gambling.States
{
    /// <summary>
    ///     勝敗判定と配当処理を行う状態。
    ///     結果を処理した後、待機状態へ戻る。
    /// </summary>
    public class ResultState : BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">ディーラーの参照</param>
        /// <param name="stateMachine">ステートマシンの参照</param>
        public ResultState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        /// <summary>
        ///     状態開始時の処理。
        /// </summary>
        public override void Enter()
        {
            // 結果発表の開始をログで通知するため
            Debug.Log("[State] Result: 結果発表");

            // 手札の点数を比較して勝敗を取得するため
            GamblingResult result = Dealer.Logic.DetermineResult();

            // 勝敗結果と賭け金に応じて、プレイヤーに配当を支払うため
            Dealer.Economy.Payout(Dealer.CurrentBetAmount, result);

            // 結果をデバッグログに出力するため
            Debug.Log($"勝負結果: {result} / 賭け金: {Dealer.CurrentBetAmount}");
            
            // 今回の賭け金額をリセットするため
            Dealer.ResetBet();

            // 再びゲームを開始できる待機状態へ戻すため
            StateMachine.ChangeState(new IdleState(Dealer, StateMachine));
        }
    }
}
