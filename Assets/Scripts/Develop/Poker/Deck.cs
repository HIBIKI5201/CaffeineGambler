using System;
using System.Collections.Generic;

namespace Develop.Poker
{
    /// <summary>
    /// 52枚（ジョーカー無し）のデッキを生成・シャッフル・供給するコンポーネント。
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// 生成と同時にデッキを初期化する。
        /// </summary>
        /// <param name="includeJoker">
        /// true にするとジョーカーを含む。Poker モードでは常に false 運用なのでデバッグ用途のみ。
        /// </param>
        public Deck(bool includeJoker = false) => Reset(includeJoker);

        private readonly List<Card> _cards = new();
        private readonly Random _random = new();

        /// <summary>現在の山札枚数。</summary>
        public int Count => _cards.Count;

        /// <summary>
        /// 山札をすべて捨てて所定のカードを積み直し、シャッフルする。
        /// </summary>
        public void Reset(bool includeJoker)
        {
            _cards.Clear();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (suit == Suit.Joker)
                {
                    // Poker モードではジョーカーを使わないため通常はここを通らない。
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
        /// 山札の末尾からカードを1枚引き抜き返す。
        /// </summary>
        public Card Draw()
        {
            var card = _cards[^1];
            _cards.RemoveAt(_cards.Count - 1);
            return card;
        }

        /// <summary>
        /// Fisher–Yates で山札全体をシャッフルする。
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