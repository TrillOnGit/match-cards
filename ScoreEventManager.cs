using System;

public static class ScoreEventManager
{
    private static int storedMaxGuesses;

    public static event Action<int>? ScoreUpdated;
    public static event Action<int>? EnergyUpdated;
    public static event Action<int>? ComboUpdated;
    public static event Action<int>? PairCountUpdated;
    public static event Action<int>? EnergySet;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }

    public static void SetMaxEnergy(int maxGuesses)
    {
        storedMaxGuesses = maxGuesses;
        EnergySet?.Invoke(maxGuesses);
    }
    public static int GetMaxGuesses()
    {
        return storedMaxGuesses;
    }
    public static void SendEnergy(int guessesChangedTo)
    {
        EnergyUpdated?.Invoke(guessesChangedTo);
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