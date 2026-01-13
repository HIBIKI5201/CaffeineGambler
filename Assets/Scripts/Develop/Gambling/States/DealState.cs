using UnityEngine;

namespace Develop.Gambling.States
{
    public class DealState : BlackJackState
    {
        public DealState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[State] Deal: カード配布中...");

            Dealer.Logic.ResetGame();
            Dealer.Logic.DealInitialCards();
            Dealer.LogHandStatus();

            ToPlayerTurn();
        }

        private void ToPlayerTurn()
        {
            StateMachine.ChangeState(new PlayerTurnState(Dealer, StateMachine));
        }
    }
}