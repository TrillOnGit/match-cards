namespace MatchCards.Effects;

public abstract class Effect
{
    public required Concentration Concentration { get; init; }
    public required Card Card { get; init; }

    public abstract void OnAdded();
    public abstract void OnRemoved();
}