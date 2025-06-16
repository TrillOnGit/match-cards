using System;
using Godot;

public partial class CardShopItemNode : Node2D
{
    [Export]
    public required CardNode CardNode { get; set; }

    [Export]
    public required Label PriceLabel { get; set; }

    public required CardShopItem Item { get; set; }

    public event Action? Clicked = null;


    public override void _EnterTree()
    {
        CardNode.CardData = Item.Card;
        CardNode.Revealed = true;
    }

    public override void _Ready()
    {
        PriceLabel.Text = $"{Item.Price}";

        CardNode.Clicked += OnCardClicked;
        Item.PriceChanged += OnPriceChanged;
    }

    public override void _ExitTree()
    {
        CardNode.Clicked -= OnCardClicked;
    }

    private void OnCardClicked()
    {
        Clicked?.Invoke();
    }

    private void OnPriceChanged(int newPrice)
    {
        PriceLabel.Text = $"{newPrice}";
    }
}