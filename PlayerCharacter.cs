using System.Collections.Generic;

interface IPlayerCharacter
{
    IEnumerable<CardData> GetCards();
}

public class IllCharacter : IPlayerCharacter
{
    public IEnumerable<CardData> GetCards() => new List<CardData>()
    {
        CardHelpers.MakeCardData(Suit.Hearts, 8),
        CardHelpers.MakeCardData(Suit.Diamonds, 2),
        CardHelpers.MakeCardData(Suit.Diamonds, 3)
    };
}

public class ElderCharacter : IPlayerCharacter
{
    public IEnumerable<CardData> GetCards() => new List<CardData>()
    {
        CardHelpers.MakeCardData(Suit.Spades, 2),
        CardHelpers.MakeCardData(Suit.Hearts, 10),
        CardHelpers.MakeCardData(Suit.Diamonds, 8)
    };
}

public class HuntedCharacter : IPlayerCharacter
{
    public IEnumerable<CardData> GetCards() => new List<CardData>()
    {
        CardHelpers.MakeCardData(Suit.Spades, 2),
        CardHelpers.MakeCardData(Suit.Hearts, 10),
        CardHelpers.MakeCardData(Suit.Diamonds, 8)
    };
}

public class AloneCharacter : IPlayerCharacter
{
    public IEnumerable<CardData> GetCards() => new List<CardData>()
    {
        CardHelpers.MakeCardData(Suit.Spades, 2),
        CardHelpers.MakeCardData(Suit.Hearts, 10),
        CardHelpers.MakeCardData(Suit.Diamonds, 8),
    };
}