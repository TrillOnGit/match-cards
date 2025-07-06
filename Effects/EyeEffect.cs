using System.Collections.Generic;
using Godot;

namespace MatchCards.Effects;

public class EyeEffect : Effect
{
    private bool AfterMatchDetected = false;
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

        if (AfterMatchDetected)
        {
            List<Card> faceDownCards = new();
            foreach (Card card in Concentration.GetCards())
            {
                if (!card.IsFaceUp)
                {
                    faceDownCards.Add(card);

                    card.Unreveal();
                }
            }
            Concentration.ShuffleCardPositions(faceDownCards);
            GD.Print("EyeEffect: After match detected, hiding face down cards and shuffling positions.");
            AfterMatchDetected = false;
        }

        if (Card == cardOne || Card == cardTwo)
        {
            foreach (Card card in Concentration.GetCards())
            {
                card.Reveal();
            }

            AfterMatchDetected = true;
        }
    }
}