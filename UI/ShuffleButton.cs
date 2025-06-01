using Godot;
using System;

public partial class ShuffleButton : Button
{
    public override void _Ready()
    {
        Text = "Shuffle";
    }

    public override void _Pressed()
    {
        MenuEventManager.SendShuffleButtonPressed();
    }
}
