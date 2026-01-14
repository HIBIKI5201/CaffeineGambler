using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Develop.Poker
{
    /// <summary>
    /// 山札の生成と手札の配布・交換を一元管理し、プレイヤー／敵双方の役判定を提供するゲームマネージャー。
    /// </summary>
    public class PokerGameManager : MonoBehaviour
    {
        /// <summary>後方互換用プロパティ。常にプレイヤー手札を返す。</summary>
        public IReadOnlyList<Card> CurrentHand => PlayerHand;

        /// <summary>プレイヤーの現在手札。</summary>
        public IReadOnlyList<Card> PlayerHand => _playerHand.Cards;

        /// <summary>敵の現在手札。</summary>
        public IReadOnlyList<Card> EnemyHand => _enemyHand.Cards;

        /// <summary>手札の所有者を識別するための列挙体。</summary>
        public enum HandOwner
        {
            Player,
            Enemy
        }

        public enum BattleResult
        {
            PlayerWin,
            EnemyWin,
            Draw
        }

        /// <summary>指定した所有者の手札を返す。</summary>
        public IReadOnlyList<Card> GetHand(HandOwner owner) =>
            owner == HandOwner.Player ? _playerHand.Cards : _enemyHand.Cards;

        /// <summary>プレイヤー手札の役を評価する（互換用）。</summary>
        public PokerRank EvaluateCurrentHand() => EvaluateHand(HandOwner.Player);

        /// <summary>指定した所有者の手札役を評価する。</summary>
        public PokerRank EvaluateHand(HandOwner owner) =>
            HandEvaluator.Evaluate(owner == HandOwner.Player ? _playerHand : _enemyHand);

        /// <summary>プレイヤー手札のみを配り直す（互換用）。</summary>
        public void DealInitialHand() => DealInitialHand(HandOwner.Player);

        /// <summary>指定した所有者の手札を配り直す。</summary>
        public void DealInitialHand(HandOwner owner)
        {
            EnsureDeckHasEnoughCards(_handSize);
            var hand = GetMutableHand(owner);
            hand.Clear();
            DrawToHand(hand, _handSize);
        }

        /// <summary>プレイヤーと敵に同時に手札を配る。</summary>
        public void DealInitialHands()
        {
            EnsureDeckHasEnoughCards(_handSize * 2);
            _playerHand.Clear();
            _enemyHand.Clear();
            DrawToHand(_playerHand, _handSize);
            DrawToHand(_enemyHand, _handSize);
        }

        /// <summary>プレイヤー手札の指定カードを交換する（互換用）。</summary>
        public void ReplaceCardAt(int index) => ReplaceCardAt(HandOwner.Player, index);

        /// <summary>指定所有者の手札でカードを1枚引き直す。</summary>
        public void ReplaceCardAt(HandOwner owner, int index)
        {
            var hand = GetMutableHand(owner);
            if (index < 0 || index >= hand.Cards.Count)
            {
                Debug.LogWarning($"ReplaceCardAt({owner}): index {index} is out of range.");
                return;
            }

            EnsureDeckHasEnoughCards(1);
            hand.Cards[index] = _deck.Draw();
        }

        /// <summary>プレイヤー手札の指定インデックス集合を交換する（互換用）。</summary>
        public void ReplaceCards(IEnumerable<int> indices) => ReplaceCards(HandOwner.Player, indices);

        /// <summary>指定所有者の手札で複数枚を一括交換する。</summary>
        public void ReplaceCards(HandOwner owner, IEnumerable<int> indices)
        {
            if (indices == null)
            {
                return;
            }

            var hand = GetMutableHand(owner);
            var targets = indices
                .Distinct()
                .Where(i => i >= 0 && i < hand.Cards.Count)
                .ToArray();

            if (targets.Length == 0)
            {
                return;
            }

            EnsureDeckHasEnoughCards(targets.Length);

            foreach (var index in targets)
            {
                hand.Cards[index] = _deck.Draw();
            }
        }

        /// <summary>プレイヤーと敵の役を比較して勝敗を返す。</summary>
        public BattleResult ResolveBattle(out PokerRank playerRank, out PokerRank enemyRank)
        {
            playerRank = EvaluateHand(HandOwner.Player);
            enemyRank = EvaluateHand(HandOwner.Enemy);

            if (playerRank > enemyRank) return BattleResult.PlayerWin;
            if (playerRank < enemyRank) return BattleResult.EnemyWin;
            return BattleResult.Draw;
        }

        [SerializeField] private bool _includeJoker;
        [SerializeField] private int _handSize = 5;

        private Deck _deck;
        private Hand _playerHand;
        private Hand _enemyHand;

        private void Awake()
        {
            _deck = new Deck(_includeJoker);
            _playerHand = new Hand();
            _enemyHand = new Hand();
        }

        /// <summary>HandOwner に応じて書き込み可能な Hand を返す。</summary>
        private Hand GetMutableHand(HandOwner owner) =>
            owner == HandOwner.Player ? _playerHand : _enemyHand;

        /// <summary>指定 Hand に山札から指定枚数を追加する。</summary>
        private void DrawToHand(Hand hand, int count)
        {
            for (var i = 0; i < count; i++)
            {
                hand.Add(_deck.Draw());
            }
        }

        /// <summary>必要枚数を満たせない場合は山札をリセットする。</summary>
        private void EnsureDeckHasEnoughCards(int requiredCards)
        {
            if (_deck.Count < requiredCards)
            {
                _deck.Reset(_includeJoker);
                Debug.Log("[Poker] Deck was reset due to insufficient cards.");
            }
        }
    }
}
