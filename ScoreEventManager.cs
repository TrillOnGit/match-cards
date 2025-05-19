using System;

public static class ScoreEventManager
{
    public static event Action<int>? ScoreUpdated;
    public static event Action<int>? GuessesUpdated;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }

    public static void SendGuesses(int guessesChangedTo)
    {
        GuessesUpdated?.Invoke(guessesChangedTo);
    }
}