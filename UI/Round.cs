using Godot;
using System;

public partial class Round : Label
{
    public int RoundCount { get; set; } = 1;
    public override void _Ready()
    {
        Text = $"Round: {RoundCount}";
        ScoreEventManager.RoundUpdated += UpdateRounds;
        ScoreEventManager.RunEnded += GameOver;
    }
    public override void _ExitTree()
    {
        ScoreEventManager.RoundUpdated -= UpdateRounds;
        ScoreEventManager.RunEnded -= GameOver;
    }

    public void UpdateRounds(int rounds)
    {
        RoundCount = rounds;
        Text = $"Round: {RoundCount}";
    }

    public void GameOver(int finalScore)
    {
        Text = $"Game Over \nFinal Score: {finalScore}";
    }
}
