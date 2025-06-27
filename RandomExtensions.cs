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
    public static int NextRankWeighted(this Random rng)
    {
        // Weights for ranks 1 to 13
        int[] weights = { 29, 26, 24, 22, 20, 19, 17, 15, 13, 10, 2, 2, 1 };
        int totalVal = 0;
        foreach (var w in weights)
        {
            totalVal += w;
        }

        int val = rng.Next(1, totalVal + 1);
        int currentTrueWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            currentTrueWeight += weights[i];
            if (val <= currentTrueWeight)
                return i + 1;
        }
        return 13;
    }

    public static CardBack NextCardBack(this Random rng)
    {
        var cardBacks = Enum.GetValues<CardBack>();
        return cardBacks[rng.Next(cardBacks.Length)];
    }
}