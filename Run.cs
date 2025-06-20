using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MatchCards.Effects;

public class Run
{
    public event Action<Concentration>? DayStarted = null;
    public event Action? DayFinished = null;
    public event Action<CardChoice>? ChoicePresented = null;
    public event Action<CardShop>? ShopPresented = null;

    private List<CardData> _deck = GetCompletedDeck(new BeastGapLocation(), new IllCharacter());
    public Dictionary<int, int> purchasedCards = new();
    private int _maxRounds = 24;
    private int _revealedCardsChange = 0;

    public int Score { get; set; } = 0;
    public int Round { get; private set; } = 1;

    private void NextRound()
    {
        Round++;
        if (Round > _maxRounds)
        {
            ScoreEventManager.SendRunOver(Score);
            return;
        }
        ScoreEventManager.SendRoundChange(Round);
    }

    public void StartDay()
    {
        var concentration = GenerateConcentration();
        concentration.GameEnded += OnConcentrationEnded;
        concentration.RevealedCardsChanged += OnRevealedCardsChanged;
        concentration.ScoreGained += ChangeScore;
        concentration.CardPermanentlyRemoved += RemoveDeckCard;
        DayStarted?.Invoke(concentration);
    }

    private void OnConcentrationEnded()
    {
        PresentShop();
    }

    private void OnRevealedCardsChanged(int change)
    {
        _revealedCardsChange += change;
        if (_revealedCardsChange < 0)
        {
            _revealedCardsChange = 0;
        }
        GD.Print("Revealed Cards Changed");
    }

    private void PresentChoice()
    {
        var choice = GenerateChoice();
        choice.ChoiceSelected += OnChoiceSelected;
        ChoicePresented?.Invoke(choice);
    }

    private void PresentShop()
    {
        var shop = GenerateShop();
        shop.ItemBought += OnItemBought;
        shop.ShoppingFinished += OnShoppingFinished;
        ShopPresented?.Invoke(shop);
    }

    private void OnItemBought(CardShopItem item)
    {
        ChangeScore(-item.Price);
        _deck.Add(item.Card);
        if (purchasedCards.TryGetValue(item.Card.Rank, out int count))
        {
            purchasedCards[item.Card.Rank] = count + 1;
        }
        else
        {
            purchasedCards[item.Card.Rank] = 1;
        }
    }

    private void RemoveDeckCard(Card card)
    {
        var cardData = card.Data;
        _deck.Remove(cardData);
    }

    private void OnShoppingFinished()
    {
        DayFinished?.Invoke();
        NextRound();
    }

    private void OnChoiceSelected(CardData card)
    {
        AddCard(card);
        DayFinished?.Invoke();
    }

    private Concentration GenerateConcentration()
    {
        var concentration = new Concentration(_deck);
        concentration.revealedCards += _revealedCardsChange;
        concentration.Layout();
        concentration.scoreOnGeneration = Score;
        return concentration;
    }

    private void ChangeScore(int score)
    {
        this.Score += score;
        ScoreEventManager.SendScoreChange(this.Score);
    }

    private CardChoice GenerateChoice() => new CardChoice();

    private CardShop GenerateShop() => new CardShop(this);

    public void AddCard(CardData card)
    {
        _deck.Add(card);
    }

    private static List<CardData> GetDefaultDeck() => new List<Suit>() { Suit.Clubs, Suit.Diamonds }
            .SelectMany(s =>

                Enumerable.Range(1, 6).Select(i =>
                {
                    var rank = i;
                    var stickers = new List<ICardSticker>();
                    if (rank == 2 && (s == Suit.Spades || s == Suit.Clubs))
                    {
                        stickers.Add(new BombSticker());
                    }
                    if (rank == 10 && s == Suit.Hearts)
                    {
                        stickers.Add(new LighterSticker());
                    }
                    if (rank == 2 && s == Suit.Diamonds)
                    {
                        stickers.Add(new StarSticker());
                    }
                    if (s == Suit.Hearts)
                    {
                        stickers.Add(new CreatureSticker());
                    }
                    if (rank == 8)
                    {
                        stickers.Add(new HunterSticker());
                    }
                    if (rank == 1 && s != Suit.Clubs)
                    {
                        stickers.Add(new KnowledgeSticker());
                    }
                    return new CardData
                    {
                        Rank = rank,
                        Suit = s,
                        // TODO: move method to this class
                        CardBack = CardManager.GetCardColor(s, i),
                        Stickers = stickers
                    };
                }
            ))
            .ToList();

    private static List<CardData> GetCompletedDeck(ILocation location, IPlayerCharacter character)
    {
        var deck = new List<CardData>();

        // Add location cards
        deck.AddRange(location.GetCards());

        // Add player character cards
        deck.AddRange(character.GetCards());

        return deck;
    }
}

public class CardChoice
{
    private List<CardData> _choices;
    public IEnumerable<CardData> Choices => _choices;

    public event Action<CardData>? ChoiceSelected = null;

    public CardChoice()
    {
        _choices = GenerateChoices();
    }

    public void Choose(CardData card)
    {
        if (!_choices.Contains(card))
        {
            throw new ArgumentOutOfRangeException(nameof(card));
        }
        ChoiceSelected?.Invoke(card);
    }

    private static List<CardData> GenerateChoices()
    {
        var rng = new Random();
        var choices = new List<CardData>();
        const int numChoices = 3;
        for (int i = 0; i < numChoices; i++)
        {
            choices.Add(GenerateRandomCard(rng));
        }
        return choices;
    }

    public static CardData GenerateRandomCard(Random rng)
    {
        var rank = rng.NextRank();
        var suit = rng.NextSuit();
        var cardBack = rng.NextCardBack();

        var stickers = new List<ICardSticker>();
        var stickerSelection = rng.Next(0, 10);
        if (stickerSelection <= 7 && suit == Suit.Spades)
        {
            stickers.Add(new BombSticker());
        }
        else if (stickerSelection <= 5 && suit == Suit.Hearts)
        {
            stickers.Add(new LighterSticker());
        }
        else if (stickerSelection <= 1 || (stickerSelection <= 4 && rank <= 3))
        {
            stickers.Add(new StarSticker());
        }
        if (rank == 1 && suit != Suit.Clubs)
        {
            stickers.Add(new KnowledgeSticker());
        }
        return new CardData()
        {
            Rank = rank,
            Suit = suit,
            CardBack = CardBack.Pink,
            Stickers = stickers
        };
    }
}