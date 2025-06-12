using System;
using Godot;

public partial class RunManager : Node
{
    [Export]
    public required PackedScene LevelScene { get; set; }

    [Export]
    public required PackedScene ChoiceScene { get; set; }

    [Export]
    public required PackedScene ShopScene { get; set; }

    private Run _run = new();

    private CardManager? _cardManager = null;

    public override void _Ready()
    {
        _run.DayStarted += OnDayStarted;
        _run.DayFinished += OnDayFinished;
        _run.ChoicePresented += OnChoicePresented;
        _run.ShopPresented += OnShopPresented;

        _run.StartDay();
    }

    public override void _ExitTree()
    {
        _run.DayStarted -= OnDayStarted;
        _run.DayFinished -= OnDayFinished;
        _run.ChoicePresented -= OnChoicePresented;
        _run.ShopPresented -= OnShopPresented;
    }

    private void OnDayStarted(Concentration concentration)
    {
        _cardManager?.QueueFree();
        _cardManager = LevelScene.Instantiate<CardManager>();
        _cardManager.Concentration = concentration;
        AddChild(_cardManager);
    }

    private void OnDayFinished()
    {
        _cardManager?.QueueFree();
        _cardManager = null;
        _run.StartDay();
    }

    private void OnChoicePresented(CardChoice cardChoice)
    {
        var choiceManager = ChoiceScene.Instantiate<ChoiceManager>();
        choiceManager.Choice = cardChoice;
        AddChild(choiceManager);
    }

    private void OnShopPresented(CardShop shop)
    {
        var shopManager = ShopScene.Instantiate<CardShopManager>();
        shopManager.CardShop = shop;
        AddChild(shopManager);
    }
}