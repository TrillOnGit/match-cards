namespace MatchCards.Effects;

public class BombEffect : Effect
{
    public override void OnAdded()
    {
        Card.Matched += OnCardMatched;
    }

    public override void OnRemoved()
    {
        Card.Matched -= OnCardMatched;
    }

    private void OnCardMatched()
    {
        foreach (var card in Concentration.GetAdjacentCards(Card))
        {
            card.Reveal();
        }
    }
}