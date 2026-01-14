using System;
using System.Collections.Generic;

namespace Develop.Poker
{
    /// <summary>
    /// ポーカー用の山札を生成・シャッフル・配る責務を持つクラス。
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// コンストラクター。作成と同時に山札を初期化する。
        /// </summary>
        /// <param name="includeJoker">ジョーカーを含めるか。</param>
        public Deck(bool includeJoker = false) => Reset(includeJoker);

        private readonly List<Card> _cards = new();
        private readonly Random _random = new();

        /// <summary>残りカード枚数。</summary>
        public int Count => _cards.Count;

        /// <summary>
        /// 山札をすべて再生成し、シャッフルする。
        /// </summary>
        /// <param name="includeJoker">ジョーカーを含めるか。</param>
        public void Reset(bool includeJoker)
        {
            _cards.Clear();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (suit == Suit.Joker)
                {
                    if (includeJoker)
                    {
                        _cards.Add(new Card { Suit = Suit.Joker, Rank = 0, IsJoker = true });
                    }

                    continue;
                }

                for (var rank = 2; rank <= 14; rank++)
                {
                    _cards.Add(new Card { Suit = suit, Rank = rank, IsJoker = false });
                }
            }

            Shuffle();
        }

        /// <summary>
        /// 山札の一番上からカードを1枚引く。
        /// </summary>
        public Card Draw()
        {
            var card = _cards[^1];
            _cards.RemoveAt(_cards.Count - 1);
            return card;
        }

        /// <summary>
        /// Fisher?Yates アルゴリズムで山札をシャッフルする。
        /// </summary>
        private void Shuffle()
        {
            for (var i = _cards.Count - 1; i > 0; i--)
            {
                var j = _random.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }
    }
}