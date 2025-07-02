namespace MatchCards.Effects;

public class RitualEffect : Effect
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
        if (cardTwo.Data.HasSticker<CorpseSticker>() && cardOne == Card)
        {
            Concentration.RemoveCardPermanent(cardTwo);
            Concentration.AddScore(20);
        }
    }
}