
using Develop.Gambling.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックの進行役（ディーラー）。
    /// Unityのコンポーネントとして存在し、状態遷移（StateMachine）とロジック、経済システム、表示層（Presenter）を繋ぐハブとなる。
    /// </summary>
    public class BlackJackDealer
    {
        public BlackJackLogic Logic => _logic;
        public GamblingEconomy Economy => _economy;
        public int CurrentBetAmount { get; set; }

        /// <summary>
        /// 依存関係を注入する初期化メソッド。
        /// Initializerから呼び出され、必要なすべてのサービスとプレゼンターをセットアップする。
        /// </summary>
        public BlackJackDealer(
            BlackJackLogic logic,
            GamblingEconomy economy,
            DealerPresenter dealerPresenter)
        {
            _logic = logic;
            _economy = economy;
            _dealerPresenter = dealerPresenter;

            _stateMachine = new BlackJackStateMachine();
            _stateMachine.Initialize(new IdleState(this, _stateMachine));
        }

        public Task WaitForInput()
        {
            _waitForResultInput = new TaskCompletionSource<bool>();
            return _waitForResultInput.Task;
        }
        public void NotifyResultInput()
        {
            // 入力待ち中であれば、待機しているTaskを完了させるため
            _waitForResultInput?.TrySetResult(true);
        }
        
        /// <summary>
        /// ディーラーの伏せカードを公開する。
        /// </summary>
        public void RevealDealerHiddenCard()
        {
            // 表示演出をプレゼンターに委譲
            _dealerPresenter.RevealDealerHiddenCard();
        }

        /// <summary>
        /// 表示されているすべてのカードをクリアする。
        /// </summary>
        public void ClearAllCardsDisplayed()
        {
            // 画面上のカードオブジェクトをクリアすることをプレゼンターに指示
            _dealerPresenter.ClearDisplayedCards();
        }

        /// <summary>
        /// ゲームを開始し、賭け金を支払う。
        /// </summary>
        /// <param name="betAmount">賭け金</param>
        public void StartGame(int betAmount)
        {
            // 現在のステート（Idleなど）にベット操作を通知
            _stateMachine.CurrentState?.OnBet(betAmount);
        }

        /// <summary>
        /// プレイヤーがカードを引く。
        /// </summary>
        public void Hit()
        {
            // 現在のステート（PlayerTurnなど）にヒット操作を通知
            _stateMachine.CurrentState?.OnHit();
        }

        /// <summary>
        /// プレイヤーがカードを引くのをやめ、勝負する。
        /// </summary>
        public void Stand()
        {
            // 現在のステート（PlayerTurnなど）にスタンド操作を通知
            _stateMachine.CurrentState?.OnStand();
        }

        /// <summary>
        /// 賭け金をリセットする。
        /// </summary>
        public void ResetBet()
        {
            // 内部で保持している賭け金額を0に戻す
            CurrentBetAmount = 0;
        }

        private TaskCompletionSource<bool> _waitForResultInput;
        private BlackJackDealer _dealer;
        private BlackJackLogic _logic;
        private GamblingEconomy _economy;
        private BlackJackStateMachine _stateMachine;
        private DealerPresenter _dealerPresenter;
    }
}
