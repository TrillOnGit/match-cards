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

    private List<CardData> _deck = GetCompletedDeck(Location.Beastgap, PlayerCharacter.Ill);

    private int revealedCardsChange = 0;

    public int Score { get; set; } = 0;

    public void StartDay()
    {
        var concentration = GenerateConcentration();
        concentration.GameEnded += OnConcentrationEnded;
        concentration.RevealedCardsChanged += OnRevealedCardsChanged;
        concentration.ScoreGained += ChangeScore;
        DayStarted?.Invoke(concentration);
    }

    private void OnConcentrationEnded()
    {
        PresentShop();
    }

    private void OnRevealedCardsChanged(int change)
    {
        revealedCardsChange += change;
        if (revealedCardsChange < 0)
        {
            revealedCardsChange = 0;
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
    }

    private void OnShoppingFinished()
    {
        DayFinished?.Invoke();
    }

    private void OnChoiceSelected(CardData card)
    {
        AddCard(card);
        DayFinished?.Invoke();
    }

    private Concentration GenerateConcentration()
    {
        var concentration = new Concentration(_deck);
        concentration.revealedCards += revealedCardsChange;
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

    private static List<CardData> GetCompletedDeck(Location location, PlayerCharacter character)
    {
        var deck = new List<CardData>();

        // Add location cards
        deck.AddRange(GetLocationCardsFor(location));

        // Add player character cards
        deck.AddRange(GetPlayerCharacterCardsFor(character));

        return deck;
    }
    private static List<CardData> GetPlayerCharacterCardsFor(PlayerCharacter character)
    {
        var cardList = new List<CardData>();

        return cardList = playerCardMap[character]
            .Select(c => new CardData
            {
                Suit = c.s,
                Rank = c.rank,
                CardBack = CardManager.GetCardColor(c.s, c.rank),
                Stickers = stickerMap.ContainsKey((c.s, c.rank))
                ? new List<ICardSticker> { stickerMap[(c.s, c.rank)] }
                : new List<ICardSticker>()
            }).ToList();

    }
    private static List<CardData> GetLocationCardsFor(Location location)
    {
        var cardList = new List<CardData>();

        return cardList = locationCardMap[location]
            .Select(c => new CardData
            {
                Suit = c.s,
                Rank = c.rank,
                CardBack = CardManager.GetCardColor(c.s, c.rank),
                Stickers = stickerMap.ContainsKey((c.s, c.rank))
                ? new List<ICardSticker> { stickerMap[(c.s, c.rank)] }
                : new List<ICardSticker>()
            }).ToList();

    }

    private static readonly Dictionary<Location, List<(Suit s, int rank)>> locationCardMap = new()
    {
        {
            Location.Beastgap, new List<(Suit suit, int rank)>
            {
                //People
                (Suit.Clubs, 1),
                (Suit.Clubs, 3),
                (Suit.Clubs, 3),
                (Suit.Clubs, 5),
                (Suit.Clubs, 7),
                (Suit.Clubs, 8),
                (Suit.Clubs, 8),
                (Suit.Clubs, 9),
                (Suit.Clubs, 9),
                (Suit.Clubs, 11),
                (Suit.Clubs, 13),
                (Suit.Clubs, 13),

                //Authority
                (Suit.Spades, 1),
                (Suit.Spades, 7),
                (Suit.Spades, 11),

                //Emotions
                (Suit.Hearts, 9),
            }
        },
        //Default Values
        {
            Location.West_Dunton, new List<(Suit suit, int rank)>
            {
                (Suit.Spades, 2),
                (Suit.Hearts, 10),
                (Suit.Diamonds, 8),
            }
        },
        {
            Location.Mount_Veil, new List<(Suit suit, int rank)>
            {
                (Suit.Spades, 2),
                (Suit.Hearts, 10),
                (Suit.Diamonds, 8),
            }
        },
    };

    private static readonly Dictionary<PlayerCharacter, List<(Suit s, int rank)>> playerCardMap = new()
    {
        {
            PlayerCharacter.Ill, new List<(Suit suit, int rank)>
            {
                //Emotions
                (Suit.Hearts, 7),

                //Items
                (Suit.Diamonds, 2),
                (Suit.Diamonds, 3),
            }
        },
        //Default Values
        {
            PlayerCharacter.Elder, new List<(Suit suit, int rank)>
            {
                (Suit.Spades, 2),
                (Suit.Hearts, 10),
                (Suit.Diamonds, 8),
            }
        },
        {
            PlayerCharacter.Hunted, new List<(Suit suit, int rank)>
            {
                (Suit.Spades, 2),
                (Suit.Hearts, 10),
                (Suit.Diamonds, 8),
            }
        },
        {
            PlayerCharacter.Alone, new List<(Suit suit, int rank)>
            {
                (Suit.Spades, 2),
                (Suit.Hearts, 10),
                (Suit.Diamonds, 8),
            }
        }
    };


    //enum of locations
    private enum Location
    {
        Beastgap,
        West_Dunton,
        Mount_Veil,
    }

    //enum of Player Characters
    private enum PlayerCharacter
    {
        Ill,
        Elder,
        Hunted,
        Alone,
    }

    private static readonly Dictionary<(Suit suit1, int rank), ICardSticker> stickerMap = new()
    {
        {(Suit.Spades, 2), new BombSticker()},
        {(Suit.Hearts, 10), new LighterSticker()},
        {(Suit.Diamonds, 2), new StarSticker()},
        {(Suit.Hearts, 8), new CreatureSticker()},
        {(Suit.Clubs, 8), new HunterSticker()},
        {(Suit.Spades, 1), new KnowledgeSticker()},
    };
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