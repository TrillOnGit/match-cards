using System;
using System.Collections.Generic;

public class CardShop
{
    private Run _run;

    private List<CardShopItem> _items;
    public IEnumerable<CardShopItem> Items => _items;

    public event Action<CardShopItem>? ItemBought;

    public event Action? ShoppingFinished;

    public int Funds => _run.Score;

    public CardShop(Run run)
    {
        _run = run;
        _items = GenerateCardShopItems();
    }

    public void Buy(CardShopItem item)
    {
        if (!_items.Contains(item))
        {
            throw new ArgumentException("Item not sold in shop", nameof(item));
        }
        if (!CanBuyItem(item))
        {
            return; // push an event for this?
        }
        ItemBought?.Invoke(item);
    }

    public void FinishShopping()
    {
        ShoppingFinished?.Invoke();
    }

    public bool CanBuyItem(CardShopItem item)
    {
        return item.Price <= Funds;
    }

    private static List<CardShopItem> GenerateCardShopItems()
    {
        var items = new List<CardShopItem>()
        {
            new(new CardData() { Suit = Suit.Spades, Rank = 1, CardBack = CardBack.Blue }, 160),
            new(new CardData() { Suit = Suit.Hearts, Rank = 2, CardBack = CardBack.Blue }, 180),
            new(new CardData() { Suit = Suit.Spades, Rank = 3, CardBack = CardBack.Blue }, 190),
            new(new CardData() { Suit = Suit.Hearts, Rank = 4, CardBack = CardBack.Blue }, 200),
            new(new CardData() { Suit = Suit.Spades, Rank = 8, CardBack = CardBack.Blue }, 210),
            new(new CardData() { Suit = Suit.Hearts, Rank = 11, CardBack = CardBack.Blue }, 230),
        };
        return items;
    }
}

public record CardShopItem(CardData Card, int Price);