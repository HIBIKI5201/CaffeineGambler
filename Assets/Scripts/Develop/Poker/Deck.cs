using System;
using System.Collections.Generic;

public class Deck
{
    public Deck(bool includeJoker = false) => Reset(includeJoker);

    private readonly List<Card> _cards = new();
    private readonly Random _random = new();

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

    public Card Draw()
    {
        var card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    private void Shuffle()
    {
        for (var i = _cards.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }
}
