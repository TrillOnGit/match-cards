using Godot;
using MatchCards.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardManager : Node
{
    [Export]
    public required PackedScene CardScene { get; set; }

    [Export]
    public required Node2D CardContainer { get; set; }

    public Concentration Concentration { get; set; } = new();

    private List<CardNode> _cardNodes = new();

    // If we flipped a card, but haven't matched yet, this variable stores that (so we highlight it)
    private Card? _lastFlipped = null;
    // If we're hovering a card, this variable stores that (for highlighting)
    private int? _hoveredRank => _hoveredCard?.Card?.IsRevealed == true ? _hoveredCard?.Card?.Data.Rank : null;

    private CardNode? _hoveredCard = null;

    public override void _Ready()
    {
        Concentration.CardAddedToBoard += OnCardAdded;
        Concentration.CardRemoved += OnCardRemoved;
        Concentration.CardPermanentlyRemoved += OnCardRemoved;
        Concentration.FirstCardFlipped += OnFirstCardFlipped;
        Concentration.MatchAttempted += OnMatchAttempted;
        MenuEventManager.ShuffleButtonPressed += ShuffleDeck;
        GD.Print($"_cardNodes count: {_cardNodes.Count}");

        Concentration.Layout();
    }

    public override void _ExitTree()
    {
        Concentration.CardAddedToBoard -= OnCardAdded;
        Concentration.CardRemoved -= OnCardRemoved;
        Concentration.CardPermanentlyRemoved -= OnCardRemoved;
        Concentration.FirstCardFlipped -= OnFirstCardFlipped;
        Concentration.MatchAttempted -= OnMatchAttempted;
        MenuEventManager.ShuffleButtonPressed -= ShuffleDeck;
    }

    public void OnCardAdded(Card card)
    {
        var cardNode = CardScene.Instantiate<CardNode>();
        cardNode.Card = card;
        // cardNode will not outlive CardManager, so it's safe to subscribe without unsubscribing
        cardNode.Clicked += () => OnCardClicked(card);
        cardNode.MouseEntered += () => OnCardMouseEntered(cardNode);
        cardNode.MouseExited += () => OnCardMouseExited(cardNode);
        CardContainer.AddChild(cardNode);
        _cardNodes.Add(cardNode);
    }

    public void OnCardRemoved(Card card)
    {
        _cardNodes.RemoveAll(c => c.Card == card);
    }

    public void OnFirstCardFlipped(Card card)
    {
        _lastFlipped = card;
        UpdateGlow();
    }

    public void OnMatchAttempted(Card card)
    {
        _lastFlipped = null;
        UpdateGlow();
    }

    private void OnCardMouseEntered(CardNode cardNode)
    {
        _hoveredCard = cardNode;
        UpdateGlow();
    }

    private void OnCardMouseExited(CardNode cardNode)
    {
        if (_hoveredCard == cardNode)
        {
            _hoveredCard = null;
        }
        UpdateGlow();
    }

    private void UpdateGlow()
    {
        foreach (var cardNode in _cardNodes)
        {
            if (cardNode.Card == _lastFlipped)
            {
                cardNode.SetGlow(CardGlowColor.Primary);
            }
            else if (cardNode.Card?.IsRevealed == true && cardNode.Card?.Data.Rank != null && cardNode.Card?.Data.Rank == _hoveredRank)
            {
                cardNode.SetGlow(CardGlowColor.Secondary);
            }
            else
            {
                cardNode.SetGlow(CardGlowColor.None);
            }
        }
    }

    public void OnCardClicked(Card card)
    {
        Concentration.Flip(card);
    }

    public void ShuffleDeck()
    {
        Concentration.EndGame();
    }

    public static CardBack GetCardColor(Suit suit)
    {
        if (suit == Suit.Clubs || suit == Suit.Spades)
        {
            return CardBack.Blue;
        }
        else
        {
            return CardBack.Red;
        }
    }
}
