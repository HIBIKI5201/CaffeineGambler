using Develop.Gambling.States;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの状態遷移を管理するクラス。
    /// Dealerから状態管理の責務を分離したもの。
    /// </summary>
    public class BlackJackStateMachine
    {
        /// <summary>
        /// 現在の状態
        /// </summary>
        public BlackJackState CurrentState { get; private set; }

        /// <summary>
        /// 初期状態を設定して開始する。
        /// </summary>
        public void Initialize(BlackJackState startState)
        {
            CurrentState = startState;
            CurrentState.Enter();
        }

        /// <summary>
        /// 状態を遷移させる。
        /// </summary>
        public void ChangeState(BlackJackState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}
