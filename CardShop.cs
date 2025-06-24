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
            GenerateCardShopItem(null, rnd.Next(1, 7), null, null),
            GenerateCardShopItem(Suit.Hearts, null, null, new List<ICardSticker> { new LighterSticker() }),
            GenerateCardShopItem(Suit.Spades, rnd.Next(2, 11), null, new List<ICardSticker> { new BombSticker() }),
            GenerateCardShopItem(Suit.Diamonds, rnd.Next(2, 9), null, new List<ICardSticker> { new StarSticker() }),
            GenerateCardShopItem(Suit.Spades, null, null, new List<ICardSticker>
            { new KnowledgeSticker(), new RightRevealSticker() }),
            GenerateCardShopItem(null, null, CardBack.Pink, null),
        };
        foreach (var item in items)
        {
            item.TimesPurchased = GetPurchasedAmount(item.Card.Rank);
        }
        return items;
    }

    //Set Null for full random range, or none in the case of stickers
    private static CardShopItem GenerateCardShopItem(Suit? suit, int? rank, CardBack? cardBack, List<ICardSticker>? stickers)
    {
        Random rnd = new();
        Suit s = suit ?? rnd.NextSuit();
        int r = rank ?? rnd.NextRank();
        CardBack b = cardBack ?? ((s == Suit.Hearts || s == Suit.Diamonds) ? CardBack.Red : CardBack.Blue);
        List<ICardSticker> st = stickers ?? new List<ICardSticker> { };

        int price = GenerateInitialPrice(s, r, b, st);

        return new CardShopItem(new CardData() { Suit = s, Rank = r, CardBack = b, Stickers = st }, price);
    }

    private static int GenerateInitialPrice(Suit suit, int rank, CardBack cardBack, List<ICardSticker> stickers)
    {
        int initPrice = 25;

        initPrice += rank / 2;

        initPrice += suit == Suit.Diamonds ? 1 : 0;

        initPrice += cardBack == CardBack.Pink ? 3 : 0;

        foreach (var sticker in stickers)
        {
            initPrice += GetStickerPrice(sticker);
        }

        return initPrice;
    }

    private static int GetStickerPrice(ICardSticker sticker)
    {
        return sticker switch
        {
            RightRevealSticker => 3,
            LeftRevealSticker => 3,
            BombSticker => 3,
            LighterSticker => 4,
            StarSticker => 6,
            KnowledgeSticker => 4,
            HunterSticker => 1,
            _ => 0,
        };
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