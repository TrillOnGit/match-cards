namespace MatchCards.Effects;

public interface IEffectData
{
    public Effect Construct(Concentration concentration, Card card);
}

public class EffectData<TEffect> : IEffectData where TEffect : Effect, new()
{
    public Effect Construct(Concentration concentration, Card card)
    {
        return new TEffect() { Concentration = concentration, Card = card };
    }
}
