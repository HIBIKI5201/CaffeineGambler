using UnityEngine;

namespace Develop.Gambling.States
{
    public class PlayerTurnState : BlackJackState
    {
        public PlayerTurnState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[State] PlayerTurn: 行動を選択してください (Hit / Stand)");
        }

        public override void OnHit()
        {
            bool isBurst = Dealer.Logic.Hit();
            Dealer.LogHandStatus();

            if (isBurst)
            {
                Debug.Log("バーストしました！");
                StateMachine.ChangeState(new ResultState(Dealer, StateMachine));
            }
            else
            {
                Debug.Log("セーフ。次の行動を選択してください。");
            }
        }

        public override void OnStand()
        {
            StateMachine.ChangeState(new DealerTurnState(Dealer, StateMachine));
        }
    }
}