using System.Threading.Tasks;

namespace Develop.Gambling.States
{
    /// <summary>
    ///     ブラックジャックの各状態を表す基底クラス。
    ///     状態遷移に必要なコンテキスト（Dealer, StateMachine）を保持する。
    /// </summary>
    public abstract class BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">データやロジックを持つContext</param>
        /// <param name="stateMachine">遷移を管理するマシン</param>
        public BlackJackState(BlackJackDealer dealer, BlackJackStateMachine stateMachine)
        {
            // 状態内部でゲームデータや遷移機能を利用できるようにするため
            Dealer = dealer;
            StateMachine = stateMachine;
        }

        /// <summary>
        ///     状態に入った際の処理。
        /// </summary>
        public virtual async Task Enter() { }

        /// <summary>
        ///     状態を抜ける際の処理。
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        ///     ベット時の振る舞い。
        /// </summary>
        /// <param name="amount">賭け金</param>
        public virtual void OnBet(int amount) { }

        /// <summary>
        ///     ヒット時の振る舞い。
        /// </summary>
        public virtual void OnHit() { }

        /// <summary>
        ///     スタンド時の振る舞い。
        /// </summary>
        public virtual Task OnStand()
        {
            return Task.CompletedTask;
        }

        protected BlackJackDealer Dealer;
        protected BlackJackStateMachine StateMachine;
    }
}
