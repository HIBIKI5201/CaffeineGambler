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
    /// UI ボタンから呼び出し、現在の役をログに出力する。
    /// </summary>
    public void LogCurrentHandRank()
    {
        if (!TryEnsureHandReady())
        {
            return;
        }

        var rank = EvaluateCurrentHand();
        var cardsText = string.Join(", ", _hand.Cards.Select(FormatCard));
        Debug.Log($"[Poker] Rank: {rank} | Cards: {cardsText}");
    }

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

    private void Start()
    {
        DealInitialHand();
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

    // 内部用: 手札が準備できているか確認する。
    private bool TryEnsureHandReady()
    {
        if (_hand == null || _hand.Cards.Count == 0)
        {
            Debug.LogWarning("Hand is empty. Call DealInitialHand before logging.");
            return false;
        }

        return true;
    }

    // 内部用: カード情報を文字列にフォーマットする。
    private static string FormatCard(Card card)
    {
        if (card.IsJoker)
        {
            return "Joker";
        }

        var rankLabel = card.Rank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => card.Rank.ToString()
        };

        return $"{card.Suit} {rankLabel}";
    }
}
