using System.Collections.Generic;

namespace MatchCards.Effects;

public class HunterEffect : Effect
{
    public override void OnAdded()
    {
        Concentration.CardsMatched += OnCardsMatched;
    }

    public override void OnRemoved()
    {
        Concentration.CardsMatched -= OnCardsMatched;
    }

    private void OnCardsMatched(Card cardOne, Card cardTwo)
    {
        if (cardOne == Card && cardTwo.Data.HasSticker<CreatureSticker>())
        {
            CreateCorpse(cardTwo);
            Concentration.RemoveCardPermanent(cardTwo);
        }
    }

    private void CreateCorpse(Card card)
    {
        var corpse = new CardData()
        {
            Suit = card.Data.Suit,
            Rank = 1,
            CardBack = card.Data.CardBack,
            Stickers = new List<ICardSticker>() {
                new CorpseSticker()
            }
        };
        Card corpseCard = new Card
        {
            X = card.X,
            Y = card.Y,
            Data = corpse,
        };

        Concentration.AddCard(corpseCard);
        corpseCard.Reveal();
    }
}