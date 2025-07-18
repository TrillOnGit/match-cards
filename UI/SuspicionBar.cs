using Godot;
using System;

public partial class SuspicionBar : ProgressBar
{
    public override void _Ready()
    {
        ScoreEventManager.SuspicionChanged += SetSuspicion;
    }

    public void SetSuspicion(int suspicion)
    {
        Value = suspicion;
    }

    public override void _ExitTree()
    {
        ScoreEventManager.SuspicionChanged -= SetSuspicion;
    }
}