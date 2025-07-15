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
        if (targetCard.Data.HasSticker<CorpseSticker>())
        {
            Concentration.RemoveCardPermanent(targetCard);
            Concentration.AddScore(20);
            Concentration.SetState(Concentration.GameState.Flipping);
            isActive = false;
        }
    }
    public void Activate()
    {
        isActive = true;
        Concentration.SetState(Concentration.GameState.Targeting);
        Card.DeActivate();
    }
}