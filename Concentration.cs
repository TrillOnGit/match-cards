using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MatchCards.Effects;

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
    private int energy = 10;
    private readonly int maxEnergy = 10;

    public event Action<Card>? CardAdded;
    public event Action<Card>? CardRemoved;

    // This event is fired when the first card of a potential pair is flipped up
    public event Action<Card>? FirstCardFlipped;

    // This event is fired when the second card of a potential pair is flipped up
    public event Action<Card>? MatchAttempted;

    // This event is fired when a pair of cards is matched
    public event Action<Card, Card>? CardsMatched;

    private List<IScoreModifier> _scoreModifiers = new();

    public void Layout(IReadOnlyCollection<CardData> faces, int width)
    {
        ClearCards();
        energy = 10;

        ScoreEventManager.SetMaxEnergy(maxEnergy);
        ScoreEventManager.SendEnergy(energy);

        var rng = new Random();
        var shuffledFaces = faces.ToArray();
        rng.Shuffle(shuffledFaces);
        int curX = 0;
        int curY = 0;
        foreach (var data in shuffledFaces)
        {
            var card = new Card(this, data)
            {
                X = curX,
                Y = curY
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
            card.Remove();
        }
    }

    public void Flip(Card card)
    {
        if (energy <= 0)
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
    }

    private void MatchPair(Card cardOne, Card cardTwo)
    {
        if (cardOne.Data.Rank == cardTwo.Data.Rank)
        {
            OnMatch(cardOne);
            OnMatch(cardTwo);
            comboCounter++;
            var scoreMod = comboCounter;
            scoreMod = ModifyScore(cardOne, cardTwo, scoreMod);
            ScoreEventManager.ComboChange(comboCounter);
            ScoreEventManager.SendScoreChange(cardOne.Data.Rank * scoreMod);
            ScoreEventManager.PairChange(1);
            CardsMatched?.Invoke(cardOne, cardTwo);
        }
        else
        {
            comboCounter = 0;
            ScoreEventManager.ComboChange(comboCounter);
            energy--;
            ScoreEventManager.SendEnergy(energy);
            cardOne.Flip(false);
            cardTwo.Flip(false);
        }
        _lastFlipped = null;
    }

    public void BurnCard(Card card)
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

    public static bool CardsAreAdjacent(Card cardOne, Card cardTwo)
    {
        return Math.Abs(cardOne.X - cardTwo.X) + Math.Abs(cardOne.Y - cardTwo.Y) == 1;
    }

    public IEnumerable<Card> GetAdjacentCards(Card card) => _cards.Where(c => CardsAreAdjacent(c, card));

    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
        CardRemoved?.Invoke(card);
        card.Remove();
    }

    public void AddScoreModifier(IScoreModifier scoreModifier)
    {
        if (!_scoreModifiers.Contains(scoreModifier))
        {
            _scoreModifiers.Add(scoreModifier);
        }
    }

    public void RemoveScoreModifier(IScoreModifier scoreModifier)
    {
        _scoreModifiers.RemoveAll(m => m == scoreModifier);
    }

    private int ModifyScore(Card cardOne, Card cardTwo, int score)
    {
        foreach (var scoreModifier in _scoreModifiers)
        {
            score = scoreModifier.ModifyScore(cardOne, cardTwo, score);
        }
        return score;
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

    private List<Effect> _effects = new();

    [SetsRequiredMembers]
    public Card(Concentration concentration, CardData data)
    {
        Data = data;
        InitializeEffectsFromData(concentration);
    }

    private void InitializeEffectsFromData(Concentration concentration)
    {
        foreach (var effectData in Data.EffectData)
        {
            var effect = effectData.Construct(concentration, this);
            effect.OnAdded();
            _effects.Add(effect);
        }
    }
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

    public void Remove()
    {
        foreach (var effect in _effects)
        {
            effect.OnRemoved();
        }
        Removed?.Invoke();
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

public enum CardBack
{
    Red,
    Blue,
    Pink,
}

public record CardData
{
    public required Suit Suit { get; set; }
    public required int Rank { get; set; }
    public required CardBack CardBack { get; set; }

    public IReadOnlyCollection<ICardSticker> Stickers { get; init; } = Array.Empty<ICardSticker>();

    public IEnumerable<IEffectData> EffectData => Stickers.OfType<IEffectData>();

    public bool HasSticker<TSticker>() where TSticker : ICardSticker => Stickers.Any(s => s is TSticker);
}

public interface IScoreModifier
{
    public int ModifyScore(Card cardOne, Card cardTwo, int score);
}