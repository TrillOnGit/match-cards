using System;

public static class RandomExtensions
{
    public static Suit NextSuit(this Random rng)
    {
        var suits = Enum.GetValues<Suit>();
        return suits[rng.Next(suits.Length)];
    }
    public static int NextRank(this Random rng)
    {
        return rng.Next(1, 11);
    }
    public static CardBack NextCardBack(this Random rng)
    {
        var cardBacks = Enum.GetValues<CardBack>();
        return cardBacks[rng.Next(cardBacks.Length)];
    }
}