namespace MatchCards.Effects;

public interface ICardSticker;

public class BombSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new BombEffect { Concentration = concentration, Card = card };
}

public class LighterSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new LighterEffect { Concentration = concentration, Card = card };
}

public class StarSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new StarEffect { Concentration = concentration, Card = card };
}

public class HunterSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new HunterEffect { Concentration = concentration, Card = card };
}

public class CreatureSticker : ICardSticker
{ }

public class CorpseSticker : ICardSticker
{ }


public class KnowledgeSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new KnowledgeEffect { Concentration = concentration, Card = card };
}

public class LeftRevealSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new LeftRevealEffect { Concentration = concentration, Card = card };
}

public class RightRevealSticker : ICardSticker, IEffectData
{
    public Effect Construct(Concentration concentration, Card card) => new RightRevealEffect { Concentration = concentration, Card = card };
}