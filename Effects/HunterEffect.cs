using System.Collections.Generic;
using Godot;

namespace MatchCards.Effects;

public class HunterEffect : Effect, IActivatable
{
    private bool activated = false;
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
        activated = true;
        Concentration.SetState(Concentration.GameState.Targeting);
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
        Concentration.SetState(Concentration.GameState.Flipping);
    }
}