using UnityEngine;

namespace Develop.Gambling.States
{
    public class IdleState : BlackJackState
    {
        public IdleState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("[State] Idle: ベット待ち");
        }

        public override void OnBet(int amount)
        {
            if (Dealer.Economy.TryBet(amount))
            {
                Dealer.CurrentBetAmount = amount;
                // マシン経由で遷移
                StateMachine.ChangeState(new DealState(Dealer, StateMachine));
            }
            else
            {
                Debug.LogWarning("資金が不足しています。");
            }
        }
    }
}