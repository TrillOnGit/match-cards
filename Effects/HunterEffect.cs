namespace MatchCards.Effects;

public class HunterEffect : Effect
{
    public override void OnAdded()
    {
        //Card.Matched += OnCardMatched;
        Concentration.CardsMatched += OnCardsMatched;
    }

    public override void OnRemoved()
    {
        //Add a corpse to the deck.
        Concentration.CardsMatched -= OnCardsMatched;
    }

    private void OnCardsMatched(Card cardOne, Card cardTwo)
    {
        if (cardOne == Card && cardTwo.Data.HasSticker<CreatureSticker>())
        {
            Concentration.RemoveCardPermanent(cardTwo);
        }
    }
}