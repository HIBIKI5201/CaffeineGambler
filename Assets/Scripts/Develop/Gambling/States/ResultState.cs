using UnityEngine;

namespace Develop.Gambling.States
{
    public class ResultState : BlackJackState
    {
        public ResultState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[State] Result: 結果発表");

            GamblingResult result = Dealer.Logic.DetermineResult();
            Dealer.Economy.Payout(Dealer.CurrentBetAmount, result);

            Debug.Log($"勝負結果: {result} / 賭け金: {Dealer.CurrentBetAmount}");
            
            Dealer.ResetBet();
            StateMachine.ChangeState(new IdleState(Dealer, StateMachine));
        }
    }
}