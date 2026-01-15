using System;
using System.Collections.Generic;
using System.Linq;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのゲーム進行（デッキ管理、手札管理）を担当するクラス。
    /// ルール判定やスコア計算はそれぞれの専門クラスに委譲する。
    /// </summary>
    public class BlackJackLogic
    {
        
        /// <summary>
        /// コンストラクタ。依存関係を外部から注入する。
        /// </summary>
        public BlackJackLogic(
            BlackJackSettings settings, 
            BlackJackScoreCalculator scoreCalculator,
            BlackJackResultDeterminer resultDeterminer)
        {
            // 設定と計算ロジックを保持
            _settings = settings;
            _scoreCalculator = scoreCalculator;
            _resultDeterminer = resultDeterminer;

            // 初期状態の構築
            _playerHand = new BlackJackHand();
            _dealerHand = new BlackJackHand();
            ResetGame();
        }
        /// <summary>
        /// カードが配られたときに発行されるイベント。
        /// </summary>
        public event Action<GamblingParticipant, Card> OnCardDealt;
        public BlackJackHand PlayerHand => _playerHand;
        public BlackJackHand DealerHand => _dealerHand;
        public bool IsGameFinished { get; private set; }

        private Deck _deck;
        private readonly BlackJackSettings _settings;
        private readonly BlackJackScoreCalculator _scoreCalculator;
        private readonly BlackJackResultDeterminer _resultDeterminer;
        private readonly BlackJackHand _playerHand;
        private readonly BlackJackHand _dealerHand;

        /// <summary>
        /// ゲームの状態をリセットする。
        /// </summary>
        public void ResetGame()
        {
            // 新しいデッキを作成しシャッフル
            _deck = new Deck();
            
            // 手札の初期化
            _playerHand.Clear();
            _dealerHand.Clear();
            IsGameFinished = false;
        }

        /// <summary>
        /// 最初のカードを配布する。
        /// </summary>
        public void DealInitialCards()
        {
            // 初期枚数分カードを追加
            for (int i = 0; i < _settings.InitialHandCount; i++)
            {
                DistributeCard(GamblingParticipant.Player);
                DistributeCard(GamblingParticipant.Dealer);
            }
        }

        /// <summary>
        /// プレイヤーがカードを1枚引く。
        /// </summary>
        /// <returns>バーストした場合は true</returns>
        public bool Hit()
        {
            if (IsGameFinished) return false;

            // カードを追加してスコア確認
            var card = DistributeCard(GamblingParticipant.Player);
            return _scoreCalculator.IsBust(_playerHand.Cards);
        }

        /// <summary>
        /// ディーラーのターンを実行する（AIの挙動）。
        /// </summary>
        public void DealerTurn()
        {
            if (IsGameFinished) return;

            // 設定された閾値（通常17点）を超えるまでカードを引き続ける
            while (_scoreCalculator.CalculateScore(_dealerHand.Cards) < _settings.DealerStandThreshold)
            {
                DistributeCard(GamblingParticipant.Dealer);
            }
        }

        /// <summary>
        /// 現在の状況から勝敗を判定する。
        /// </summary>
        /// <returns>ゲーム結果</returns>
        public GamblingResult DetermineResult()
        {
            IsGameFinished = true;
            // 判定ロジックを専門クラスに委譲
            return _resultDeterminer.DetermineResult(_playerHand, _dealerHand);
        }

        /// <summary>
        /// 手札のスコアを計算する（外部公開用）。
        /// </summary>
        public int CalculateScore(IEnumerable<Card> cards)
        {
            // 計算を計算機に委譲
            return _scoreCalculator.CalculateScore(cards);
        }
        
        /// <summary>
        /// デッキからカードを引き、指定された手札に追加してイベントを発行する。
        /// </summary>
        private Card DistributeCard(GamblingParticipant participant)
        {
            var card = _deck.Draw();
            switch (participant)
            {
                case GamblingParticipant.Player:
                    _playerHand.AddCard(card);
                    break;
                case GamblingParticipant.Dealer:
                    _dealerHand.AddCard(card);
                    break;
            }
            OnCardDealt?.Invoke(participant, card);
            return card;
        }
    }
}