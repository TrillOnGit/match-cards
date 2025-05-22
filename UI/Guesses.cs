using Godot;
using System;

public partial class Guesses : Label
{
    public int GuessesCount { get; set; } = 10;
    public int MaxGuesses { get; set; } = 10;
    public override void _Ready()
    {
        Text = $"Guesses: {GuessesCount} / {MaxGuesses}";
        ScoreEventManager.GuessesUpdated += UpdateGuesses;
    }
    public override void _ExitTree()
    {
        ScoreEventManager.GuessesUpdated -= UpdateGuesses;
    }
    public void UpdateGuesses(int guesses)
    {
        GuessesCount = MaxGuesses - guesses;
        Text = $"Guesses: {GuessesCount} / {MaxGuesses}";
        if (GuessesCount == 0)
        {
            Text = $"Guesses: {GuessesCount} / {MaxGuesses} \nGame Over";
        }
    }
}
