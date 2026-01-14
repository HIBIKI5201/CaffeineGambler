using System;
using System.Collections.Generic;
using System.Linq;

namespace Develop.Gambling
{
    /// <summary>
    /// 52枚1組のカードデッキを管理するクラス。
    /// </summary>
    public class Deck
    {
        private readonly Stack<Card> _cards;

        /// <summary>
        /// 新しいデッキを作成し、シャッフルする。
        /// </summary>
        public Deck()
        {
            // 全52種類のカードをリストとして生成
            var allCards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    allCards.Add(new Card(suit, rank));
                }
            }

            // Fisher-Yatesアルゴリズムでリストをシャッフル
            var random = new Random();
            for (int i = allCards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (allCards[i], allCards[j]) = (allCards[j], allCards[i]);
            }
            
            // シャッフルしたリストをStackに詰める
            _cards = new Stack<Card>(allCards);
        }

        /// <summary>
        /// デッキからカードを1枚引く。
        /// </summary>
        /// <returns>引いたカード</returns>
        /// <exception cref="InvalidOperationException">デッキが空の場合</exception>
        public Card Draw()
        {
            if (_cards.Count > 0)
            {
                return _cards.Pop();
            }
            throw new InvalidOperationException("デッキが空です。");
        }

        public int Count => _cards.Count;
    }
}
