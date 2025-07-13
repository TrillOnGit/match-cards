using System.Collections.Generic;
using Godot;

namespace MatchCards.Effects;

public class HunterEffect : Effect, IActivatable
{
    private bool activated = false;
    public override void OnAdded()
    {
        //Concentration.CardsMatched += OnCardsMatched;
        Card.Activated += Activate;
        Concentration.CardTargeted += OnCardTargeted;
    }

    public override void OnRemoved()
    {
        //Concentration.CardsMatched -= OnCardsMatched;
        Card.Activated -= Activate;
        Concentration.CardTargeted -= OnCardTargeted;
    }

    // private void OnCardsMatched(Card cardOne, Card cardTwo)
    // {
    //     if (cardOne == Card && cardTwo.Data.HasSticker<CreatureSticker>())
    //     {
    //         CreateCorpse(cardTwo);
    //         Concentration.RemoveCardPermanent(cardTwo);
    //     }
    // }

    private void CreateCorpse(Card card)
    {
        var corpse = new CardData()
        {
            Suit = card.Data.Suit,
            Rank = 2,
            CardBack = card.Data.CardBack,
            Stickers = new List<ICardSticker>() {
                new CorpseSticker()
            }
        };
        Card corpseCard = new()
        {
            X = card.X,
            Y = card.Y,
            Data = corpse,
        };

        Concentration.AddCard(corpseCard);
        Concentration.AddCardToDecklist(corpseCard);
        corpseCard.Reveal();
        GD.Print("Corpse created for targeted creature.");
    }

    public void Activate()
    {
        activated = true;
        Concentration.SetStateTargeting();
        GD.Print("Hunter activated, waiting for target.");
    }

    public void OnCardTargeted(Card targetCard)
    {
        if (!activated)
        {
            return;
        }
        if (targetCard.Data.HasSticker<CreatureSticker>() && activated)
        {
            CreateCorpse(targetCard);
            Concentration.RemoveCardPermanent(targetCard);
        }
        Concentration.SetStateFlipping();
        GD.Print("Card Targeted.");
    }
}