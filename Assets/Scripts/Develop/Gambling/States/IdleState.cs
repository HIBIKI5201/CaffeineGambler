using System.Threading.Tasks;
using UnityEngine;

namespace Develop.Gambling.States
{
    /// <summary>
    ///     ゲーム開始前の待機状態。
    ///     プレイヤーのベット入力を待ち受ける。
    /// </summary>
    public class IdleState : BlackJackState
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="dealer">ディーラーの参照</param>
        /// <param name="stateMachine">ステートマシンの参照</param>
        public IdleState(BlackJackDealer dealer, BlackJackStateMachine stateMachine) : base(dealer, stateMachine) { }

        /// <summary>
        ///     状態開始時の処理。
        /// </summary>
        public override async Task Enter()
        {
            Dealer.ClearAllCardsDisplayed();
            // 現在の状態をデバッグログで把握できるようにするため
            Debug.Log("[State] Idle: ベット待ち");
        }

        /// <summary>
        ///     ベットが行われた際の処理。
        /// </summary>
        /// <param name="amount">賭け金</param>
        public override void OnBet(int amount)
        {
            // 経済システムを通じて支払いが可能か確認するため
            if (Dealer.Economy.TryBet(amount))
            {
                // 配当計算のために現在のベット額を記録するため
                Dealer.CurrentBetAmount = amount;

                // 支払い完了後、カード配布フェーズへ移行するため
                StateMachine.ChangeState(new DealState(Dealer, StateMachine));
            }
            else
            {
                // 資金不足でゲームを開始できないことをユーザー（開発者）に知らせるため
                Debug.LogWarning("資金が不足しています。");
            }
        }
    }
}
