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
        item.CardPurchased();

        AdjustSameRankPrices(item.Card.Rank);
    }

    private void AdjustSameRankPrices(int rank)
    {
        foreach (var item in _items)
        {
            if (item.Card.Rank == rank)
            {
                item.TimesPurchased = GetPurchasedAmount(rank);
                item.SetCost(rank);
            }
        }
    }

    public void FinishShopping()
    {
        ShoppingFinished?.Invoke();
    }

    public bool CanBuyItem(CardShopItem item)
    {
        return item.Price <= Funds;
    }

    private List<CardShopItem> GenerateCardShopItems()
    {
        Random rnd = new();
        var items = new List<CardShopItem>()
        {
            new(new CardData() { Suit = Suit.Spades, Rank = 1, CardBack = CardBack.Blue }, 0),
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
        foreach (var item in items)
        {
            item.Card.Rank = rnd.Next(1, 14);
            item.TimesPurchased = GetPurchasedAmount(item.Card.Rank);
        }
        return items;
    }

    private int GetPurchasedAmount(int rank)
    {
        if (_run.purchasedCards.TryGetValue(rank, out int amount))
        {
            return amount;
        }
        return 0;
    }
}

public class CardShopItem
{
    public CardData Card { get; }
    public int Price { get; private set; }
    public int TimesPurchased { get; set; } = 0;
    private readonly int BasePrice;
    public event Action<int>? PriceChangedForRank;
    public event Action<int>? PriceSet;
    public CardShopItem(CardData card, int price)
    {
        Card = card;
        Price = price;
        PriceChangedForRank += SetCost;
        BasePrice = price;
    }
    public void SetCost(int rank)
    {
        if (rank != Card.Rank)
        {
            return;
        }
        Price = GetPrice();
        PriceSet?.Invoke(Price);
    }

    public void CardPurchased()
    {
        TimesPurchased++;
        PriceChangedForRank?.Invoke(Card.Rank);
    }

    public int GetPrice()
    {
        return BasePrice + (TimesPurchased * 5);
    }
}