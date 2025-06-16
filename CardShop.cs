using System;
using System.Collections.Generic;
using MatchCards.Effects;

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
        item.IncreasePrice(5);
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
        Random rnd = new();
        var items = new List<CardShopItem>()
        {
            new(new CardData() { Suit = Suit.Spades, Rank = rnd.Next(1, 14), CardBack = CardBack.Blue }, 0),
            new(new CardData() { Suit = Suit.Hearts, Rank = rnd.Next(1, 14), CardBack = CardBack.Red,
            Stickers = new List<ICardSticker> {new LighterSticker()} }, 18),
            new(new CardData() { Suit = Suit.Spades, Rank = rnd.Next(2, 11), CardBack = CardBack.Blue,
            Stickers = new List<ICardSticker> {new BombSticker()} }, 19),
            new(new CardData() { Suit = Suit.Diamonds, Rank = rnd.Next(2, 9), CardBack = CardBack.Red,
            Stickers = new List<ICardSticker> {new StarSticker()} }, 20),
            new(new CardData() { Suit = Suit.Spades, Rank = rnd.Next(1, 14), CardBack = CardBack.Blue,
            Stickers = new List<ICardSticker> {new KnowledgeSticker()} }, 21),
            new(new CardData() { Suit = Suit.Clubs, Rank = rnd.Next(11, 14), CardBack = CardBack.Pink }, 13),
        };
        return items;
    }
}

public class CardShopItem
{
    public CardData Card { get; }
    public int Price { get; private set; }
    public event Action<int>? PriceChanged;
    public CardShopItem(CardData card, int price)
    {
        Card = card;
        Price = price;
    }

    public void IncreasePrice(int amount)
    {
        Price += amount;
        PriceChanged?.Invoke(Price);
    }
}