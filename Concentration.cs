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
    private readonly int maxGuesses = 10;

    public event Action<Card>? CardAdded;
    public event Action<Card>? CardRemoved;

    // This event is fired when the first card of a potential pair is flipped up
    public event Action<Card>? FirstCardFlipped;

    // This event is fired when the second card of a potential pair is flipped up
    public event Action<Card>? MatchAttempted;

    public void Layout(IReadOnlyCollection<CardData> faces, int width)
    {
        ClearCards();

        ScoreEventManager.SetMaxGuesses(maxGuesses);

        var rng = new Random();
        var shuffledFaces = faces.ToArray();
        rng.Shuffle(shuffledFaces);
        int curX = 0;
        int curY = 0;
        foreach (var data in shuffledFaces)
        {
            Card card = new Card()
            {
                X = curX,
                Y = curY,
                Data = data,
            };
            AddCard(card);
            curX++;
            if (curX >= width)
            {
                curY++;
                curX = 0;
            }
        }

        // Reveal a number of cards at random
        var revealNumber = 10;
        var randomPositions = Enumerable.Range(0, faces.Count).ToArray();
        rng.Shuffle(randomPositions);
        for (int i = 0; (i < revealNumber) && (i < _cards.Count); i++)
        {
            _cards[randomPositions[i]].Reveal();
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
        if (guesses >= maxGuesses)
        {
            card.Reveal();
            return;
        }
        if (card.IsFaceUp || card.IsBurning)
        {
            return;
        }
        card.Flip(true);
        card.Reveal();
        if (_lastFlipped == null)
        {
            _lastFlipped = card;
            FirstCardFlipped?.Invoke(card);
            return;
        }

        var burnList = _cards.Where(c => c.IsBurning).ToList();
        foreach (var c in burnList)
        {
            foreach (var otherCard in _cards)
            {
                if (CardsAreAdjacent(c, otherCard) && otherCard.Data.Rank < c.Data.Rank)
                {
                    // If a burning card is adjacent to a lower rank card, burn the lower rank card
                    BurnCard(otherCard);
                }
            }
        }
        MatchAttempted?.Invoke(card);
        MatchPair(_lastFlipped, card);
    }

    public Card? GetCardAtPos(int X, int Y) =>
        _cards.SingleOrDefault(c => c.X == X && c.Y == Y);

    public IEnumerable<Card> GetCards() => _cards;

    private void OnMatch(Card card)
    {
        card.Match();
        if (card.Data.IsBomb)
        {
            foreach (var otherCard in _cards)
            {
                if (CardsAreAdjacent(card, otherCard))
                {
                    otherCard.Reveal();
                }
            }
        }
        if (card.Data.IsLighter)
        {
            BurnCard(card);
        }
    }

    private void MatchPair(Card cardOne, Card cardTwo)
    {
        if (cardOne.Data.Rank == cardTwo.Data.Rank)
        {
            OnMatch(cardOne);
            OnMatch(cardTwo);
            comboCounter++;
            var scoreMod = comboCounter;
            foreach (var starCard in _cards)
            {
                if (starCard.Data.IsStar && starCard.IsFaceUp)
                {
                    scoreMod *= 2; // Double score for stars
                }
            }
            ScoreEventManager.ComboChange(comboCounter);
            ScoreEventManager.SendScoreChange(cardOne.Data.Rank * scoreMod);
            ScoreEventManager.PairChange(1);
        }
        else
        {
            comboCounter = 0;
            ScoreEventManager.ComboChange(comboCounter);
            guesses++;
            ScoreEventManager.SendGuesses(guesses);
            cardOne.Flip(false);
            cardTwo.Flip(false);
        }
        _lastFlipped = null;
    }

    private void BurnCard(Card card)
    {
        card.Burn();
        foreach (var otherCard in _cards)
        {
            if (CardsAreAdjacent(card, otherCard))
            {
                otherCard.Reveal();
            }
        }
    }

    private bool CardsAreAdjacent(Card cardOne, Card cardTwo)
    {
        return Math.Abs(cardOne.X - cardTwo.X) + Math.Abs(cardOne.Y - cardTwo.Y) == 1;
    }
}


public record Card
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsFaceUp { get; private set; } = false;
    public bool IsRevealed { get; private set; } = false;
    public bool IsBurning { get; private set; } = false;
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

    public void Burn()
    {
        IsBurning = true;
        Burned?.Invoke();
    }

    public void Match()
    {
        Matched?.Invoke();
    }

    public event Action<bool>? Flipped;
    public event Action? Revealed;
    public event Action? Removed;
    public event Action? Burned;
    public event Action? Matched;
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
    public bool IsBomb { get; set; } = false;
    public bool IsLighter { get; set; } = false;
    public bool IsStar { get; set; } = false;
}