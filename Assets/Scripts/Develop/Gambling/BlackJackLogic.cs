using System;
using System.Collections.Generic;
using System.Linq;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックのゲームロジックを担当するクラス。
    /// ScriptableObject から設定を読み込み、カードの配布や勝敗判定を行う。
    /// </summary>
    public class BlackJackLogic
    {
        /// <summary>
        /// カードが配られたときに発行されるイベント。
        /// 第1引数: 配られた対象 (Player/Dealer)
        /// 第2引数: 配られたカードの情報
        /// </summary>
        public event Action<GamblingParticipant, Card> OnCardDealt;
        
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="settings">ゲーム設定データ</param>
        public BlackJackLogic(BlackJackSettings settings)
        {
            // 設定を保持
            _settings = settings;
            ResetGame();
        }

        public IReadOnlyList<Card> PlayerHand => _playerHand;
        public IReadOnlyList<Card> DealerHand => _dealerHand;
        public bool IsGameFinished { get; private set; }

        private Deck _deck;
        private readonly BlackJackSettings _settings;
        private List<Card> _playerHand;
        private List<Card> _dealerHand;

        /// <summary>
        /// ゲームの状態をリセットする。
        /// </summary>
        public void ResetGame()
        {
            // 新しいデッキを作成しシャッフル
            _deck = new Deck();
            
            // 手札の初期化
            _playerHand = new List<Card>();
            _dealerHand = new List<Card>();
            IsGameFinished = false;
        }

        /// <summary>
        /// 最初の2枚ずつのカードを配る。
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
        /// プレイヤーがカードを引く（ヒット）。
        /// </summary>
        /// <returns>バーストした場合は true</returns>
        public bool Hit()
        {
            if (IsGameFinished) return false;

            // カードを追加してスコア確認
            DistributeCard(GamblingParticipant.Player);

            if (CalculateScore(_playerHand) > _settings.TargetScore)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ディーラーのターンを実行する。
        /// </summary>
        public void DealerTurn()
        {
            if (IsGameFinished) return;

            // 閾値を超えるまで引き続ける
            while (CalculateScore(_dealerHand) < _settings.DealerStandThreshold)
            {
                DistributeCard(GamblingParticipant.Dealer);
            }
        }

        /// <summary>
        /// 最終的な勝敗を判定する。
        /// </summary>
        /// <returns>勝敗結果</returns>
        public GamblingResult DetermineResult()
        {
            IsGameFinished = true;

            int playerScore = CalculateScore(_playerHand);
            int dealerScore = CalculateScore(_dealerHand);

            // プレイヤーのバースト判定
            if (playerScore > _settings.TargetScore)
            {
                return GamblingResult.Lose;
            }

            // ディーラーのバースト判定
            if (dealerScore > _settings.TargetScore)
            {
                if (IsBlackJack(_playerHand)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }

            // 点数比較
            if (playerScore > dealerScore)
            {
                if (IsBlackJack(_playerHand)) return GamblingResult.BlackJack;
                return GamblingResult.Win;
            }
            else if (playerScore < dealerScore)
            {
                return GamblingResult.Lose;
            }
            else
            {
                return GamblingResult.Draw;
            }
        }

        /// <summary>
        /// 手札の点数を計算する。
        /// </summary>
        /// <param name="hand">計算対象の手札</param>
        /// <returns>計算された点数</returns>
        public int CalculateScore(IReadOnlyList<Card> hand)
        {
            int score = 0;
            int aceCount = hand.Count(card => card.Rank == Rank.Ace);

            foreach (var card in hand)
            {
                if (card.Rank == Rank.Ace)
                {
                    // Aは一旦11点として計算
                    score += _settings.AceBonusScore; 
                }
                else if (card.Rank >= Rank.Ten)
                {
                    // 10, J, Q, Kは一律設定された点数（10点）
                    score += _settings.FaceCardScore;
                }
                else
                {
                    score += (int)card.Rank;
                }
            }

            // 目標点数を超えている場合、Aを1点として扱い直す
            while (score > _settings.TargetScore && aceCount > 0)
            {
                score -= (_settings.AceBonusScore - 1); // 11点から1点にするため10引く
                aceCount--;
            }

            return score;
        }
        
        /// <summary>
        /// 指定された対象にカードを1枚配り、イベントを発行する。
        /// </summary>
        /// <param name="participant">カードを配る対象</param>
        private void DistributeCard(GamblingParticipant participant)
        {
            var card = _deck.Draw();
            switch (participant)
            {
                case GamblingParticipant.Player:
                    _playerHand.Add(card);
                    break;
                case GamblingParticipant.Dealer:
                    _dealerHand.Add(card);
                    break;
            }
            OnCardDealt?.Invoke(participant, card);
        }

        /// <summary>
        /// 手札がブラックジャックかどうか判定する。
        /// </summary>
        private bool IsBlackJack(IReadOnlyList<Card> hand)
        {
            // 枚数と点数が設定通りならブラックジャック
            return hand.Count == _settings.InitialHandCount && CalculateScore(hand) == _settings.TargetScore;
        }
    }
}