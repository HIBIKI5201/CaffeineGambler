using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 山札・手札・役判定を統括し、プレイサイクルを進行させる。
/// </summary>
public class PokerGameManager : MonoBehaviour
{
    /// <summary>現在の手札（読み取り専用）。</summary>
    public IReadOnlyList<Card> CurrentHand => _hand.Cards;
    [SerializeField] private bool _includeJoker;
    [SerializeField] private int _handSize = 5;
    private Deck _deck;
    private Hand _hand;

    /// <summary>現在の手札を評価して役を返す。</summary>
    public PokerRank EvaluateCurrentHand() => HandEvaluator.Evaluate(_hand);

    /// <summary>
    /// 新しいラウンド用に手札を配り直す。
    /// </summary>
    public void DealInitialHand()
    {
        EnsureDeckHasEnoughCards(_handSize);
        _hand.Clear();
        DrawToHand(_handSize);
    }

    /// <summary>
    /// 指定インデックスのカードを1枚引き直す。
    /// </summary>
    public void ReplaceCardAt(int index)
    {
        if (index < 0 || index >= _hand.Cards.Count)
        {
            Debug.LogWarning($"ReplaceCardAt: index {index} is out of range.");
            return;
        }

        EnsureDeckHasEnoughCards(1);
        _hand.Cards[index] = _deck.Draw();
    }

    /// <summary>
    /// 指定インデックス集合のカードを一度に引き直す。
    /// </summary>
    public void ReplaceCards(IEnumerable<int> indices)
    {
        if (indices == null)
        {
            return;
        }

        var targets = indices
            .Distinct()
            .Where(i => i >= 0 && i < _hand.Cards.Count)
            .ToArray();

        if (targets.Length == 0)
        {
            return;
        }

        EnsureDeckHasEnoughCards(targets.Length);

        foreach (var index in targets)
        {
            _hand.Cards[index] = _deck.Draw();
        }
    }

    private void Awake()
    {
        _deck = new Deck(_includeJoker);
        _hand = new Hand();
    }

    // 内部用: 手札に指定枚数のカードを引く。
    private void DrawToHand(int count)
    {
        for (var i = 0; i < count; i++)
        {
            _hand.Add(_deck.Draw());
        }
    }

    // 内部用: 山札に指定枚数以上のカードがなければリセットする。
    private void EnsureDeckHasEnoughCards(int requiredCards)
    {
        if (_deck.Count < requiredCards)
        {
            _deck.Reset(_includeJoker);
            Debug.Log("[Poker] Deck was reset due to insufficient cards.");
        }
    }
}
