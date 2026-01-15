using System.Threading.Tasks;
using UnityEngine;
namespace Develop.Gambling.States
{
    /// <summary>
    ///     プレイヤーの行動選択状態。
    ///     Hit または Stand の入力を待機する。
    /// </summary>
    public class PlayerTurnState : BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">ディーラーの参照</param>
        /// <param name="stateMachine">ステートマシンの参照</param>
        public PlayerTurnState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        /// <summary>
        ///     状態開始時の処理。
        /// </summary>
        public override void Enter()
        {
            // プレイヤーに入力を促すメッセージを表示するため
            Debug.Log("[State] PlayerTurn: 行動を選択してください (Hit / Stand)");
        }

        /// <summary>
        ///     ヒット（カードを引く）時の処理。
        /// </summary>
        public override void OnHit()
        {
            // ロジックを通じてカードを引き、バーストしたか結果を受け取るため
            bool isBurst = Dealer.Logic.Hit();

            if (isBurst)
            {
                // バーストした場合は即座にゲーム終了（結果表示）へ移行するため
                Debug.Log("バーストしました！");
                StateMachine.ChangeState(new ResultState(Dealer, StateMachine));
            }
            else
            {
                // まだバーストしていなければ、続けて行動を選択させるため
                Debug.Log("セーフ。次の行動を選択してください。");
            }
        }

        /// <summary>
        ///     スタンド（勝負する）時の処理。
        /// </summary>
        public override async Task OnStand()
        {
            Dealer.RevealDealerHiddenCard();
            // ディーラーの裏向きのカードを表にし、少し待ってから次のステートへ移行するため
            await Task.Delay(1000);

            StateMachine.ChangeState(new DealerTurnState(Dealer, StateMachine));
        }


    }
}
