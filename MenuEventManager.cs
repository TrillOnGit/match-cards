using System;

public static class MenuEventManager
{
    public static event Action? ShuffleButtonPressed;

    public static void SendShuffleButtonPressed()
    {
        ShuffleButtonPressed?.Invoke();
    }
}