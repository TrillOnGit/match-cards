using System.Linq;
using Godot;

public partial class ChoiceManager : Node
{
    public CardChoice Choice { get; set; } = new();

    public override void _Ready()
    {
        Choice.ChoiceSelected += OnChoiceSelected;
        // TODO: display choices
        Choice.Choose(Choice.Choices.First());
    }

    public override void _ExitTree()
    {
        Choice.ChoiceSelected -= OnChoiceSelected;
    }

    private void OnChoiceSelected(CardData cardData)
    {
        QueueFree();
    }
}