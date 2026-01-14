using Develop.Gambling.States;

namespace Develop.Gambling
{
    /// <summary>
    ///     ブラックジャックの状態遷移を管理するクラス。
    ///     Dealerから状態管理の責務を分離したもの。
    /// </summary>
    public class BlackJackStateMachine
    {
        /// <summary>
        ///     現在の状態。
        /// </summary>
        public BlackJackState CurrentState { get; private set; }

        /// <summary>
        ///     初期状態を設定して開始する。
        /// </summary>
        /// <param name="startState">最初に開始する状態</param>
        public void Initialize(BlackJackState startState)
        {
            // 初期状態をセットするため
            CurrentState = startState;

            // 状態に応じた初期化処理を実行するため
            CurrentState.Enter();
        }

        /// <summary>
        ///     状態を遷移させる。
        /// </summary>
        /// <param name="newState">次に遷移する状態</param>
        public void ChangeState(BlackJackState newState)
        {
            // 前の状態の終了処理を呼び出すため
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            // 新しい状態に更新するため
            CurrentState = newState;

            // 新しい状態の開始処理を呼び出すため
            CurrentState.Enter();
        }
    }
}