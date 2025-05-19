using System;

public static class ScoreEventManager
{
    public static event Action<int>? ScoreUpdated;

    public static void SendScoreChange(int scoreChangedBy)
    {
        ScoreUpdated?.Invoke(scoreChangedBy);
    }
}