using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ChoiceManager : Node
{
    [Export]
    public required PackedScene CardScene { get; set; }

    [Export]
    public required Node2D CardContainer { get; set; }

    [Export]
    public float Spacing { get; set; } = 200f;

    public CardChoice Choice { get; set; } = new();

    private List<CardNode> _cardNodes = new();

    public override void _Ready()
    {
        Choice.ChoiceSelected += OnChoiceSelected;
        int index = 0;
        foreach (var card in Choice.Choices)
        {
            AddChoice(card, index++);
        }
    }

    public override void _ExitTree()
    {
        Choice.ChoiceSelected -= OnChoiceSelected;
    }

    private void AddChoice(CardData cardData, int choiceIndex)
    {
        var cardNode = CardScene.Instantiate<CardNode>();
        cardNode.CardData = cardData;
        cardNode.Clicked += () => Choice.Choose(cardData);
        cardNode.Position = new Vector2(Spacing * choiceIndex, 0f);
        cardNode.Revealed = true;
        CardContainer.AddChild(cardNode);
    }

    private void OnChoiceSelected(CardData cardData)
    {
        QueueFree();
    }
}