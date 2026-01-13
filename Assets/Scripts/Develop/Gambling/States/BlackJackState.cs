namespace Develop.Gambling.States
{
    /// <summary>
    /// ブラックジャックの各状態を表す基底クラス。
    /// </summary>
    public abstract class BlackJackState
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="dealer">データやロジックを持つContext</param>
        /// <param name="stateMachine">遷移を管理するマシン</param>
        public BlackJackState(BlackJackDealer dealer, BlackJackStateMachine stateMachine)
        {
            Dealer = dealer;
            StateMachine = stateMachine;
        }

        protected BlackJackDealer Dealer;
        protected BlackJackStateMachine StateMachine;

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void OnBet(int amount) { }
        public virtual void OnHit() { }
        public virtual void OnStand() { }
    }
}