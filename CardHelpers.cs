using System.Collections.Generic;
using MatchCards.Effects;

public static class CardHelpers
{
    public static CardData MakeCardData(Suit suit, int rank) => new CardData
    {
        Suit = suit,
        Rank = rank,
        CardBack = CardManager.GetCardColor(suit, rank),
        Stickers = stickerMap.ContainsKey((suit, rank))
                ? new List<ICardSticker> { stickerMap[(suit, rank)] }
                : new List<ICardSticker>()
    };

    private static readonly Dictionary<(Suit suit1, int rank), ICardSticker> stickerMap = new()
    {
        {(Suit.Spades, 2), new BombSticker()},
        {(Suit.Hearts, 10), new LighterSticker()},
        {(Suit.Diamonds, 2), new RitualSticker()},
        {(Suit.Hearts, 8), new CreatureSticker()},
        {(Suit.Clubs, 8), new HunterSticker()},
        {(Suit.Spades, 1), new KnowledgeSticker()},
    };
}