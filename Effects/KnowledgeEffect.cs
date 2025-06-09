namespace MatchCards.Effects;

public class KnowledgeEffect : Effect
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
        Concentration.ModifyRevealCardsBy(1);
    }
}