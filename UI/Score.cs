using Godot;
using System;

public partial class Score : Label
{
    public int Points { get; set; } = 0;
    public override void _Ready()
    {
        Text = "Score: 0";
        ScoreEventManager.ScoreUpdated += UpdateScore;
    }
    public void UpdateScore(int score)
    {
        Points += score;
        Text = $"Score: {Points}";
    }
}