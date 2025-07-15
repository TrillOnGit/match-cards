using System.Collections.Generic;
using Godot;

namespace MatchCards.Effects;

public class HunterEffect : Effect, IActivatable
{
    private bool isActive = false;
    public override void OnAdded()
    {
        Card.Activated += Activate;
        Concentration.CardTargeted += OnCardTargeted;
    }

    public override void OnRemoved()
    {
        Card.Activated -= Activate;
        Concentration.CardTargeted -= OnCardTargeted;
    }

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
    }

    public void Activate()
    {
        isActive = true;
        Concentration.SetState(Concentration.GameState.Targeting);
        Card.DeActivate();
    }

    public void OnCardTargeted(Card targetCard)
    {
        if (isActive)
        {
            if (targetCard.Data.HasSticker<CreatureSticker>())
            {
                CreateCorpse(targetCard);
                Concentration.RemoveCardPermanent(targetCard);
                Card.DeActivate();
            }
            Concentration.SetState(Concentration.GameState.Flipping);
            isActive = false;
        }
    }
}