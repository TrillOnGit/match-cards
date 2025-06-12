using System;
using System.Collections.Generic;
using Godot;

public partial class CardShopManager : Node2D
{
    [Export]
    public required PackedScene CardShopItemScene { get; set; }

    [Export]
    public required Node2D CardShopItemContainer { get; set; }

    [Export]
    public required Button FinishButton { get; set; }

    [Export]
    public float Spacing { get; set; } = 200f;

    public required CardShop CardShop { get; set; }

    public override void _Ready()
    {
        CardShop.ShoppingFinished += OnShoppingFinished;
        int index = 0;
        foreach (var item in CardShop.Items)
        {
            AddItem(item, index++);
        }
        FinishButton.Pressed += OnFinishButtonPressed;
    }

    private void OnFinishButtonPressed()
    {
        CardShop.FinishShopping();
    }

    public override void _ExitTree()
    {
        CardShop.ShoppingFinished -= OnShoppingFinished;
    }

    private void AddItem(CardShopItem cardShopItem, int choiceIndex)
    {

        var cardShopItemNode = CardShopItemScene.Instantiate<CardShopItemNode>();
        cardShopItemNode.Item = cardShopItem;
        cardShopItemNode.Clicked += () => CardShop.Buy(cardShopItem);
        cardShopItemNode.Position = new Vector2(Spacing * choiceIndex, 0f);
        CardShopItemContainer.AddChild(cardShopItemNode);
    }

    private void OnShoppingFinished()
    {
        QueueFree();
    }
}