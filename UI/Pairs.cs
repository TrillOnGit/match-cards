using Godot;
using System;

public partial class Pairs : Label
{
    public int PairsCount { get; set; } = 0;
    public int PairMax { get; set; } = 8;

    public override void _Ready()
    {
        Text = $"Pairs: {PairsCount} / {PairMax}";
        ScoreEventManager.PairCountUpdated += UpdatePairs;
    }

    public void UpdatePairs(int pairs)
    {
        PairsCount += pairs;
        Text = $"Pairs: {PairsCount} / {PairMax}";
    }
}