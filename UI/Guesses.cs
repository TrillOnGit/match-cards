using Godot;
using System;

public partial class Guesses : Label
{
    public int GuessesCount { get; set; }
    public int MaxGuesses { get; set; }
    public override void _Ready()
    {
        MaxGuesses = ScoreEventManager.GetMaxGuesses();
        GuessesCount = MaxGuesses;

        Text = $"Guesses: {GuessesCount} / {MaxGuesses}";
        ScoreEventManager.GuessesUpdated += UpdateGuesses;
        ScoreEventManager.GuessesSet += SetMaxGuesses;
    }
    public override void _ExitTree()
    {
        ScoreEventManager.GuessesUpdated -= UpdateGuesses;
        ScoreEventManager.GuessesSet -= SetMaxGuesses;
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
    public void SetMaxGuesses(int maxGuesses)
    {
        GD.Print($"Setting max guesses to {maxGuesses}");
        MaxGuesses = maxGuesses;
        GuessesCount = MaxGuesses;
        Text = $"Guesses: {GuessesCount} / {MaxGuesses}";
    }
}
