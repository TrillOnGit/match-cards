using System;

public static class ScoreEventManager
{
    public static event Action<int>? ScoreUpdated;
    public static event Action<int>? GuessesUpdated;
    public static event Action<int>? ComboUpdated;
    public static event Action<int>? PairCountUpdated;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }

    public static void SendGuesses(int guessesChangedTo)
    {
        GuessesUpdated?.Invoke(guessesChangedTo);
    }

    public static void ComboChange(int comboChangedBy)
    {
        ComboUpdated?.Invoke(comboChangedBy);
    }

    public static void PairChange(int pairsChangedBy)
    {
        PairCountUpdated?.Invoke(pairsChangedBy);
    }
}