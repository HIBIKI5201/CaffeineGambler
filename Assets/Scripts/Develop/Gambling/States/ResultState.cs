using UnityEngine;

namespace Develop.Gambling.States
{
    /// <summary>
    /// 結果発表状態。
    /// 勝敗を確定し、配当を行い、ゲームを終了する。
    /// </summary>
    public class ResultState : BlackJackState
    {
        public ResultState(BlackJackDealer dealer) : base(dealer) { }

        public override void Enter()
        {
            Debug.Log("[State] Result: 結果発表");

            // 勝敗判定
            GamblingResult result = Dealer.Logic.DetermineResult();
            
            // 配当処理
            Dealer.Economy.Payout(Dealer.CurrentBetAmount, result);

            Debug.Log($"勝負結果: {result} / 賭け金: {Dealer.CurrentBetAmount}");
            
            // リセットして待機状態へ戻る
            // 将来的には「もう一度遊ぶ？」などのダイアログを出して待機する形になるかもしれない
            Dealer.ResetBet();
            Dealer.ChangeState(new IdleState(Dealer));
        }
    }
}
