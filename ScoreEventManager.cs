using System;

public static class ScoreEventManager
{
    private static int storedMaxGuesses;

    public static event Action<int>? ScoreUpdated;
    public static event Action<int>? GuessesUpdated;
    public static event Action<int>? ComboUpdated;
    public static event Action<int>? PairCountUpdated;
    public static event Action<int>? GuessesSet;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }

    public static void SetMaxGuesses(int maxGuesses)
    {
        storedMaxGuesses = maxGuesses;
        GuessesSet?.Invoke(maxGuesses);
    }
    public static int GetMaxGuesses()
    {
        return storedMaxGuesses;
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