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

    private List<CardData> _deck = GetDefaultDeck();

    private int revealedCardsChange = 0;
    //Add the event listening for when revealed cards number changes

    public void StartDay()
    {
        var concentration = GenerateConcentration();
        concentration.GameEnded += OnConcentrationEnded;
        concentration.RevealedCardsChanged += OnRevealedCardsChanged;
        DayStarted?.Invoke(concentration);
    }

    private void OnConcentrationEnded()
    {
        PresentChoice();
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

    private void OnChoiceSelected(CardData card)
    {
        AddCard(card);
        DayFinished?.Invoke();
    }

    private Concentration GenerateConcentration()
    {
        var concentration = new Concentration(_deck);
        concentration.revealedCards += revealedCardsChange;
        concentration.Layout(9);
        return concentration;
    }

    private CardChoice GenerateChoice() => new CardChoice();

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
                    if (rank == 1 && s == Suit.Diamonds)
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
                    if (rank == 1)
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

    private static CardData GenerateRandomCard(Random rng)
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
        if (rank == 1)
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