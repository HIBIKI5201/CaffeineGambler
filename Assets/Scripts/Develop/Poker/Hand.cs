using System.Collections.Generic;

/// <summary>
/// プレイヤーの手札を保持・操作するクラス。
/// </summary>
public class Hand
{
    /// <summary>現在の手札。</summary>
    public List<Card> Cards { get; } = new();

    /// <summary>手札をすべて捨てる。</summary>
    public void Clear() => Cards.Clear();

    /// <summary>手札にカードを1枚追加する。</summary>
    public void Add(Card card) => Cards.Add(card);
}