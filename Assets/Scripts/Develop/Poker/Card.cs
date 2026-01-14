/// <summary>
/// ポーカーで使用する1枚のカード情報を表す構造体。
/// </summary>
public struct Card
{
    /// <summary>カードのスート。</summary>
    public Suit Suit;

    /// <summary>カードのランク。2?14（J=11, Q=12, K=13, A=14）、ジョーカーは0。</summary>
    public int Rank;

    /// <summary>ジョーカーかどうか。</summary>
    public bool IsJoker;
}

/// <summary>
/// カードに割り当てられるスートの種類。
/// </summary>
public enum Suit
{
    Spade,
    Heart,
    Diamond,
    Club,
    Joker
}
