using System.Collections.Generic;

public class Hand
{
    public List<Card> Cards { get; } = new();

    public void Clear() => Cards.Clear();
    public void Add(Card card) => Cards.Add(card);
}