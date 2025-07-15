namespace MatchCards.Effects;

public class LighterEffect : Effect, IActivatable
{
    public bool isActive = false;
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
    public void Activate()
    {
        isActive = true;
        Concentration.SetState(Concentration.GameState.Targeting);
    }
    public void OnCardTargeted(Card card)
    {
        if (isActive)
        {
            Concentration.BurnCard(card);
            Concentration.SetState(Concentration.GameState.Flipping);
            isActive = false;
        }
    }
}