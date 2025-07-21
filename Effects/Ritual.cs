using System;

namespace MatchCards.Effects;

public class RitualEffect : Effect, IActivatable
{
    private bool isActive = false;
    public override void OnAdded()
    {
        Concentration.CardTargeted += OnCardTargeted;
        Card.Activated += Activate;
    }

    public override void OnRemoved()
    {
        Concentration.CardTargeted -= OnCardTargeted;
        Card.Activated -= Activate;
    }

    private void OnCardTargeted(Card targetCard)
    {
        if (isActive)
        {
            if (targetCard.Data.HasSticker<CorpseSticker>())
            {
                Concentration.RemoveCardPermanent(targetCard);
                AddScoringCard(targetCard);
            }
            Concentration.SetState(Concentration.GameState.Flipping);
            isActive = false;
        }
    }

    private void AddScoringCard(Card targetCard)
    {
        Random rng = new();
        var queen = new CardData()
        {
            Suit = targetCard.Data.Suit,
            Rank = rng.Next(11, 14), // Queen rank
            CardBack = targetCard.Data.CardBack
        };
        Card newCard = new()
        {
            X = targetCard.X,
            Y = targetCard.Y,
            Data = queen,
        };

        Concentration.AddCard(newCard);
        Concentration.AddCardToDecklist(newCard);
        newCard.Reveal();
    }
    public void Activate()
    {
        isActive = true;
        Concentration.SetState(Concentration.GameState.Targeting);
        Card.DeActivate();
    }
}