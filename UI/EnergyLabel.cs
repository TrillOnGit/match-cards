using Godot;
using System;

public partial class EnergyLabel : Label
{
    public int Energy { get; set; }
    public int MaxEnergy { get; set; }
    public override void _Ready()
    {
        MaxEnergy = ScoreEventManager.GetMaxEnergy();
        Energy = MaxEnergy;

        Text = $"Energy: {Energy}";
        ScoreEventManager.EnergyUpdated += UpdateEnergy;
        ScoreEventManager.EnergySet += SetMaxEnergy;
    }
    public override void _ExitTree()
    {
        ScoreEventManager.EnergyUpdated -= UpdateEnergy;
        ScoreEventManager.EnergySet -= SetMaxEnergy;
    }
    public void UpdateEnergy(int energy)
    {
        Energy = energy;
        Text = $"Energy: {Energy}";
        if (Energy == 0)
        {
            Text = $"Energy: {Energy} \nRound Over";
        }
    }
    public void SetMaxEnergy(int maxEnergy)
    {
        GD.Print($"Setting max energy to {maxEnergy}");
        MaxEnergy = maxEnergy;
        Energy = MaxEnergy;
        Text = $"Energy: {Energy}";
    }
}
