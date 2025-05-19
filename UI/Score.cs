using Godot;
using System;

public partial class Score : Label
{
    public int Points { get; set; } = 0;
    public override void _Ready()
    {
        // Set the initial score to 0
        Text = "Score: 0";

        ScoreEventManager.ScoreUpdated += UpdateScore;
    }
    public void UpdateScore(int score)
    {
        // Update the score text
        Points += score;

        Text = $"Score: {Points}";
    }
}