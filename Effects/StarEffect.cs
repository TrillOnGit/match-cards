namespace MatchCards.Effects;

public class StarEffect : Effect, IScoreModifier
{
    public int ModifyScore(Card cardOne, Card cardTwo, int score)
    {
        if (Card.IsFaceUp && (Concentration.CardsAreAdjacent(Card, cardOne) || Concentration.CardsAreAdjacent(Card, cardTwo)))
        {
            return score * 2;
        }
        return score;
    }

    public override void OnAdded()
    {
        Concentration.AddScoreModifier(this);
    }

    public override void OnRemoved()
    {
        Concentration.RemoveScoreModifier(this);
    }
}