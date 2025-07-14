using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Godot;
using MatchCards.Effects;

interface IConcentration
{
    IEnumerable<Card> GetCards();
    Card? GetCardAtPos(int X, int Y);
    void Flip(Card card);
}

public class Concentration : IConcentration
{
    private readonly IReadOnlyCollection<CardData> _faces;
    private readonly List<Card> _cards = new();
    // In concentration, you flip a card face up and then try another card.
    // This variable contains the first card, or null after you flip a pair.
    private Card? _lastFlipped;
    private int comboCounter = 0;
    private int energy = 12;
    private int progress = 0;
    private readonly int maxEnergy = 12;
    public int revealedCards = 3;
    public int scoreOnGeneration = 0;
    public GameState state = GameState.Flipping;

    public event Action<Card>? CardAddedToBoard;
    public event Action<CardData>? AddDeckCard;
    public event Action<Card>? CardRemoved;
    public event Action<Card>? CardPermanentlyRemoved;

    // This event is fired when the first card of a potential pair is flipped up
    public event Action<Card>? FirstCardFlipped;

    // This event is fired when the second card of a potential pair is flipped up
    public event Action<Card>? MatchAttempted;

    // This event is fired when a pair of cards is matched
    public event Action<Card, Card>? CardsMatched;

    // This event is fired when you run out of guesses or flip every flippable card
    public event Action? GameEnded;

    // This event is fired when the number of revealed cards at roundstart changes
    public event Action<int>? RevealedCardsChanged;

    public event Action<int>? ScoreGained;
    public event Action<Card>? CardTargeted;

    private List<IScoreModifier> _scoreModifiers = new();

    public Concentration() : this(Array.Empty<CardData>()) { }

    public Concentration(IReadOnlyCollection<CardData> cardData)
    {
        _faces = cardData;
    }
    public void Layout()
    {
        ClearCards();
        energy = maxEnergy;

        ScoreEventManager.SetMaxEnergy(maxEnergy);
        ScoreEventManager.SendEnergy(energy);
        ScoreEventManager.SendScoreChange(scoreOnGeneration);

        var width = GetLayoutWidth(_faces.Count);

        var rng = new Random();
        var shuffledFaces = _faces.ToArray();
        rng.Shuffle(shuffledFaces);
        int curX = 0;
        int curY = 0;
        foreach (var data in shuffledFaces)
        {
            var card = new Card()
            {
                X = curX,
                Y = curY,
                Data = data
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
        var randomPositions = Enumerable.Range(0, _cards.Count).ToArray();
        rng.Shuffle(randomPositions);
        for (int i = 0; (i < revealedCards) && (i < _cards.Count); i++)
        {
            _cards[randomPositions[i]].Reveal();
        }
    }

    public static void ShuffleCardPositions(List<Card> cards)
    {
        var rng = new Random();
        var positions = cards.Select(card => (card.X, card.Y)).ToArray();

        rng.Shuffle(positions);

        for (int i = 0; i < positions.Length; i++)
        {
            cards[i].Move(positions[i].X, positions[i].Y);
        }
    }

    public void SwapCards(Card cardOne, Card cardTwo)
    {
        cardOne.Move(cardTwo.X, cardTwo.Y);
        cardTwo.Move(cardOne.X, cardOne.Y);
    }

    public void LayoutWithoutResets()
    {
        ClearCards();

        var width = GetLayoutWidth(_faces.Count);

        var rng = new Random();
        var shuffledFaces = _faces.ToArray();
        rng.Shuffle(shuffledFaces);
        int curX = 0;
        int curY = 0;
        foreach (var data in shuffledFaces)
        {
            var card = new Card()
            {
                X = curX,
                Y = curY,
                Data = data
            };
            AddCard(card);
            curX++;
            if (curX >= width)
            {
                curY++;
                curX = 0;
            }
        }
    }

    private static int GetLayoutWidth(int cardCount) => cardCount switch
    {
        <= 9 => 3,
        <= 16 => 4,
        <= 20 => 5,
        <= 24 => 6,
        <= 28 => 7,
        <= 32 => 8,
        _ => 9
    };

    private bool IsGameOver() => energy <= 0 || !_cards.Any(c => c.IsFlippable);

    private void CheckGameOver()
    {
        if (IsGameOver())
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        foreach (var card in _cards)
        {
            card.Reveal();
        }
        SetState(GameState.Night);
        GameEnded?.Invoke();
    }

    public void AddCard(Card card)
    {
        _cards.Add(card);
        //Send signal for adding a card to run
        card.InitializeEffects(this);
        CardAddedToBoard?.Invoke(card);
    }

    public void AddCardToDecklist(Card card)
    {
        AddDeckCard?.Invoke(card.Data);
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
        if (!card.IsFlippable)
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
            foreach (var boardCard in _cards)
            {
                if (boardCard.IsBurning)
                {
                    continue;
                }
                if (CardsAreAdjacent(c, boardCard) && boardCard.Data.Rank <= c.Data.Rank)
                {
                    // If a burning card is adjacent to a lower or equal rank card, burn the card
                    BurnCard(boardCard);
                }
            }
        }
        MatchAttempted?.Invoke(card);
        MatchPair(_lastFlipped, card);
        CheckGameOver();
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
            var scoreVal = GetCardMatchValue(cardOne, cardTwo);

            OnMatch(cardOne);
            OnMatch(cardTwo);

            comboCounter++;
            ScoreEventManager.ComboChange(comboCounter);
            energy -= 1;
            ScoreEventManager.SendEnergy(energy);

            AddScore(scoreVal);
            CardsMatched?.Invoke(cardOne, cardTwo);
            GetAndSetProgressState();
        }
        else
        {
            comboCounter = 0;
            ScoreEventManager.ComboChange(comboCounter);
            energy = Math.Max(energy - 2, 0);
            ScoreEventManager.SendEnergy(energy);
            cardOne.Flip(false);
            cardTwo.Flip(false);
        }
        _lastFlipped = null;
    }

    private int GetCardMatchValue(Card cardOne, Card cardTwo)
    {
        var initialScore = (cardOne.Data.Rank > 10 || cardTwo.Data.Rank > 10) ? 0
        : Math.Max(cardOne.Data.Rank, cardTwo.Data.Rank);
        var scoreMult = 1;
        var scoreAdd = comboCounter;

        scoreMult = ModifyScore(cardOne, cardTwo, scoreMult);


        return (initialScore + scoreAdd) * scoreMult;

    }

    public void AddScore(int increase)
    {
        ScoreGained?.Invoke(increase);
    }

    public void GetAndSetProgressState()
    {
        var jackMatched = false;
        var queenMatched = false;
        var kingMatched = false;
        foreach (var card in _cards)
        {
            if (card.Data.Rank == 11 && card.IsFaceUp)
            {
                jackMatched = true;
            }
            else if (card.Data.Rank == 12 && card.IsFaceUp)
            {
                queenMatched = true;
            }
            else if (card.Data.Rank == 13 && card.IsFaceUp)
            {
                kingMatched = true;
            }
        }
        SetProgress(
            (jackMatched ? 33 : 0) +
            (queenMatched ? 33 : 0) +
            (kingMatched ? 34 : 0)
        );
    }

    public void SetProgress(int value)
    {
        progress = value;
        ScoreEventManager.SendProgressChanged(progress);

        if (progress >= 100)
        {
            //Win Game
            GD.Print("You win");
        }
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

    public static bool CardIsDirectLeftwards(Card checkCard, Card triggerCard)
    {
        return (checkCard.Y == triggerCard.Y) && (checkCard.X < triggerCard.X);
    }

    public IEnumerable<Card> GetDirectLeftCards(Card card) => _cards.Where(c => CardIsDirectLeftwards(c, card));

    public static bool CardIsDirectRightwards(Card checkCard, Card triggerCard)
    {
        return (checkCard.Y == triggerCard.Y) && (checkCard.X > triggerCard.X);
    }

    public IEnumerable<Card> GetDirectRightCards(Card card) => _cards.Where(c => CardIsDirectRightwards(c, card));

    public void RemoveCardPermanent(Card card)
    {
        _cards.Remove(card);
        CardPermanentlyRemoved?.Invoke(card);
        card.Remove();
    }

    public IEnumerable<Card> GetSameRankCards(Card card) => _cards.Where(c => CardIsSameRank(c, card));

    public static bool CardIsSameRank(Card checkCard, Card triggerCard)
    {
        if (checkCard != triggerCard && checkCard.Data.Rank == triggerCard.Data.Rank)
        {
            return true;
        }
        return false;
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

    public void ModifyRevealCardsBy(int change)
    {
        if (change == 0)
        {
            return;
        }
        RevealedCardsChanged?.Invoke(change);
    }

    public void SendCardTargeted(Card target)
    {
        CardTargeted?.Invoke(target);
        GD.Print("Card Target Sent: " + target.Data.Rank + " of " + target.Data.Suit);
    }

    public void SelectCard(Card card)
    {
        switch (state)
        {
            case GameState.Flipping:
                if (card.IsActivatable) card.Activate();
                Flip(card);
                break;
            case GameState.Targeting:
                SendCardTargeted(card);
                break;
            case GameState.Night:
                GD.Print("Cannot select cards at night.");
                break;
        }
    }

    public void SetState(GameState newState)
    {
        state = newState;
        GD.Print("Concentration state set to: " + state);
    }

    public enum GameState
    {
        Flipping,
        Targeting,
        Night,
    }
}


public record Card
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsFaceUp { get; private set; } = false;
    public bool IsRevealed { get; private set; } = false;
    public bool IsBurning { get; private set; } = false;
    public bool IsActivatable { get; private set; } = false;

    public bool IsFlippable => !IsFaceUp && !IsBurning;

    public required CardData Data { get; init; }

    private List<Effect> _effects = new();

    /// <summary>
    /// Initializes game-logic effects of this Card to operate within an
    /// ongoing <see cref="Concentration"/> game.
    /// 
    /// This includes triggers, passive effects, score modifiers, etc.
    /// </summary>
    /// <param name="concentration"></param>
    public void InitializeEffects(Concentration concentration)
    {
        foreach (var effectData in Data.EffectData)
        {
            var effect = effectData.Construct(concentration, this);
            effect.OnAdded();
            _effects.Add(effect);
        }
    }
    public void Activate()
    {
        if (!IsFlippable)
        {
            Activated?.Invoke();
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
    public void Move(int x, int y)
    {
        X = x;
        Y = y;
        Moved?.Invoke(X, Y);
    }

    public void Reveal()
    {
        if (IsRevealed) return;
        IsRevealed = true;
        Revealed?.Invoke();
    }

    public void Unreveal()
    {
        if (!IsRevealed) return;
        IsRevealed = false;
        Hidden?.Invoke();
    }

    public void Burn()
    {
        IsActivatable = false;
        IsBurning = true;
        Reveal();
        Flip(true);
        Burned?.Invoke();
    }

    public void Match()
    {
        IsActivatable = true;
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
    public event Action? Hidden;
    public event Action? Removed;
    public event Action? Burned;
    public event Action? Matched;
    public event Action<int, int>? Moved;
    public event Action? Activated;
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

public interface IActivatable
{
    public void Activate();
}