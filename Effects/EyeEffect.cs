using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MatchCards.Effects;

public class EyeEffect : Effect, IActivatable
{
    private bool activatable = true;
    public override void OnAdded()
    {
        Card.Activated += Activate;
    }

    public override void OnRemoved()
    {
        Card.Activated -= Activate;
    }

    public void Activate()
    {
        if (activatable)
        {
            Concentration.ShuffleCardPositions(Concentration.GetCards().ToList());
            activatable = false;
        }
    }
}