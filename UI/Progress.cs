using Godot;
using System;

public partial class Progress : ProgressBar
{
    public override void _Ready()
    {
        ScoreEventManager.ProgressChanged += SetProgress;
    }

    public void SetProgress(int progress)
    {
        Value = progress;
    }

    public override void _ExitTree()
    {
        ScoreEventManager.ProgressChanged -= SetProgress;
    }
}
