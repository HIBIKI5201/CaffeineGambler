using UnityEngine;

namespace Develop.Gambling.States
{
    public class DealerTurnState : BlackJackState
    {
        public DealerTurnState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[State] DealerTurn: ディーラーのドロー処理");

            Dealer.Logic.DealerTurn();
            Dealer.LogHandStatus();

            StateMachine.ChangeState(new ResultState(Dealer, StateMachine));
        }
    }
}