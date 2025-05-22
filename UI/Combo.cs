using Godot;
using System;

public partial class Combo : Label
{
    public int ComboCount { get; set; } = 0;
    public override void _Ready()
    {
        Text = "Combo: 0";
        ScoreEventManager.ComboUpdated += UpdateCombo;
    }
    public override void _ExitTree()
    {
        ScoreEventManager.ComboUpdated -= UpdateCombo;
    }
    public void UpdateCombo(int combo)
    {
        ComboCount = combo;
        Text = $"Combo: {ComboCount}";
    }
}
