using System.Collections.Generic;

interface ILocation
{
    IEnumerable<CardData> GetCards();
}

public class BeastGapLocation : ILocation
{
    public IEnumerable<CardData> GetCards() => new List<CardData>
    {
        //People
        CardHelpers.MakeCardData(Suit.Clubs, 1),
        CardHelpers.MakeCardData(Suit.Clubs, 8),
        CardHelpers.MakeCardData(Suit.Clubs, 8),
        CardHelpers.MakeCardData(Suit.Clubs, 10),

        //Authority
        CardHelpers.MakeCardData(Suit.Spades, 1),
        CardHelpers.MakeCardData(Suit.Spades, 10),

        //Emotions
        //CardHelpers.MakeCardData(Suit.Hearts, 8),
    };
}

public class WestDuntonLocation : ILocation
{
    public IEnumerable<CardData> GetCards() => new List<CardData>
    {
        CardHelpers.MakeCardData(Suit.Spades, 2),
        CardHelpers.MakeCardData(Suit.Hearts, 10),
        CardHelpers.MakeCardData(Suit.Diamonds, 8),
    };
}

public class MountVeilLocation : ILocation
{
    public IEnumerable<CardData> GetCards() => new List<CardData>
    {
        CardHelpers.MakeCardData(Suit.Spades, 2),
        CardHelpers.MakeCardData(Suit.Hearts, 10),
        CardHelpers.MakeCardData(Suit.Diamonds, 8),
    };
}