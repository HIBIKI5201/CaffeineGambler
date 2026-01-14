namespace Develop.Gambling
{
    /// <summary>
    /// スートとランクを持つカードのデータ構造。
    /// </summary>
    public readonly struct Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Suit}の{Rank}";
        }
    }
}
