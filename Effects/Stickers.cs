namespace MatchCards.Effects;

public interface ICardSticker : IEffectData;

public class BombSticker : ICardSticker
{
    public Effect Construct(Concentration concentration, Card card) => new BombEffect { Concentration = concentration, Card = card };
}

public class LighterSticker : ICardSticker
{
    public Effect Construct(Concentration concentration, Card card) => new LighterEffect { Concentration = concentration, Card = card };
}

public class StarSticker : ICardSticker
{
    public Effect Construct(Concentration concentration, Card card) => new StarEffect { Concentration = concentration, Card = card };
}
