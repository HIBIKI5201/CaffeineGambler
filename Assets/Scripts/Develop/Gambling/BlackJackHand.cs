using System.Collections.Generic;

namespace Develop.Gambling
{
    /// <summary>
    /// ブラックジャックにおける一組の手札を管理するクラス。
    /// 状態の保持に特化し、特定のゲームルールからは独立している。
    /// </summary>
    public class BlackJackHand
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public BlackJackHand()
        {
            // 手札を格納するリストを初期化するため
            _cards = new List<Card>();
        }

        /// <summary>
        /// 手札に含まれるカードの読み取り専用リスト。
        /// </summary>
        public IReadOnlyList<Card> Cards => _cards;

        /// <summary>
        /// 手札にカードを追加する。
        /// </summary>
        /// <param name="card">追加するカード</param>
        public void AddCard(Card card)
        {
            // 外部から取得したカードを手札リストに格納するため
            _cards.Add(card);
        }

        /// <summary>
        /// 手札を空にする。
        /// </summary>
        public void Clear()
        {
            // 次のゲーム開始時に手札をリセットする必要があるため
            _cards.Clear();
        }

        private readonly List<Card> _cards;
    }
}
