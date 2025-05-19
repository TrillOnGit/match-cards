using Godot;
using System;

public partial class Guesses : Label
{
    public int GuessesCount { get; set; } = 6;
    public override void _Ready()
    {
        Text = "Guesses: 6 / 6";
        ScoreEventManager.GuessesUpdated += UpdateGuesses;
    }
    public void UpdateGuesses(int guesses)
    {
        GuessesCount = 6 - guesses;
        Text = $"Guesses: {GuessesCount} / 6";
        if (GuessesCount == 0)
        {
            Text = $"Guesses: {GuessesCount} / 6 \nGame Over";
        }
    }
}
