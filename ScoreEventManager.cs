using System;

public static class ScoreEventManager
{
    private static int storedMaxEnergy;

    public static event Action<int>? ScoreUpdated;
    public static event Action<int>? EnergyUpdated;
    public static event Action<int>? ComboUpdated;
    public static event Action<int>? EnergySet;
    public static event Action<int>? RoundUpdated;
    public static event Action<int>? RunEnded;
    public static event Action<int>? ProgressChanged;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }

    public static void SetMaxEnergy(int maxEnergy)
    {
        storedMaxEnergy = maxEnergy;
        EnergySet?.Invoke(maxEnergy);
    }
    public static int GetMaxEnergy()
    {
        return storedMaxEnergy;
    }
    public static void SendEnergy(int energyChangedTo)
    {
        EnergyUpdated?.Invoke(energyChangedTo);
    }

    public static void ComboChange(int comboChangedBy)
    {
        ComboUpdated?.Invoke(comboChangedBy);
    }

    public static void SendRoundChange(int roundChangedTo)
    {
        RoundUpdated?.Invoke(roundChangedTo);
    }

    public static void SendRunOver(int Score)
    {
        RunEnded?.Invoke(Score);
    }

    public static void SendProgressChanged(int progress)
    {
        ProgressChanged?.Invoke(progress);
    }
}