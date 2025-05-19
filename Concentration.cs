using System;
using System.Collections.Generic;
using System.Linq;

interface IConcentration
{
    IEnumerable<Card> GetCards();
    Card? GetCardAtPos(int X, int Y);
    void Flip(Card card);
}

public class Concentration : IConcentration
{
    private readonly List<Card> _cards = new();
    // In concentration, you flip a card face up and then try another card.
    // This variable contains the first card, or null after you flip a pair.
    private Card? _lastFlipped;
    private int comboCounter = 0;
    private int guesses = 0;

    public event Action<Card>? CardAdded;
    public event Action<Card>? CardRemoved;

    public void Layout(IReadOnlyCollection<CardData> faces)
    {
        ClearCards();

        var rng = new Random();
        var shuffledFaces = faces.ToArray();
        rng.Shuffle(shuffledFaces);

        var num = faces.Count;

        // We'll try to lay the cards out in a square
        int width = (int)Math.Ceiling(Math.Sqrt(num));

        int curX = 0;
        int curY = 0;
        foreach (var data in shuffledFaces)
        {
            AddCard(new Card()
            {
                X = curX,
                Y = curY,
                Data = data
            });
            curX++;
            if (curX >= width)
            {
                curY++;
                curX = 0;
            }
        }
    }

    private void AddCard(Card card)
    {
        _cards.Add(card);
        CardAdded?.Invoke(card);
    }

    private void ClearCards()
    {
        var cards = _cards.ToList();
        _cards.Clear();
        foreach (var card in cards)
        {
            CardRemoved?.Invoke(card);
        }
    }

    public void Flip(Card card)
    {
        if (card.IsFaceUp)
        {
            return;
        }
        card.Flip(true);
        card.Reveal();
        if (_lastFlipped == null)
        {
            _lastFlipped = card;
            return;
        }
        MatchPair(_lastFlipped, card);
    }

    public Card? GetCardAtPos(int X, int Y) =>
        _cards.SingleOrDefault(c => c.X == X && c.Y == Y);

    public IEnumerable<Card> GetCards() => _cards;

    private void MatchPair(Card cardOne, Card cardTwo)
    {
        if (cardOne.Data.Rank == cardTwo.Data.Rank)
        {
            if (guesses < 10)
            {
                comboCounter++;
                ScoreEventManager.ComboChange(comboCounter);
                ScoreEventManager.SendScoreChange(cardOne.Data.Rank * comboCounter);
            }
        }
        else
        {
            comboCounter = 0;
            ScoreEventManager.ComboChange(comboCounter);
            if (guesses < 10)
            {
                guesses++;
                ScoreEventManager.SendGuesses(guesses);
            }
            cardOne.Flip(false);
            cardTwo.Flip(false);
        }
        _lastFlipped = null;
    }
}

public record Card
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsFaceUp { get; set; } = false;
    public bool IsRevealed { get; set; } = false;
    public required CardData Data { get; init; }

    public void Flip(bool faceUp)
    {
        if (faceUp != IsFaceUp)
        {
            IsFaceUp = faceUp;
            Flipped?.Invoke(IsFaceUp);
        }
    }

    public void Reveal()
    {
        if (IsRevealed) return;
        IsRevealed = true;
        Revealed?.Invoke();
    }

    public event Action<bool>? Flipped;
    public event Action? Revealed;
    public event Action? Removed;
}

public enum Suit
{
    Spades,
    Hearts,
    Clubs,
    Diamonds
}

public record CardData
{
    public Suit Suit { get; set; }
    public int Rank { get; set; }
    public CardBack CardBack { get; set; }
}