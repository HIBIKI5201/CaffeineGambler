public struct Card
{
    public Suit Suit;
    public int Rank;     // 2?14, Joker = 0
    public bool IsJoker;
}

public enum Suit
{
    Spade,
    Heart,
    Diamond,
    Club,
    Joker
}
