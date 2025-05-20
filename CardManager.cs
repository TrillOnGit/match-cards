using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardManager : Node
{
    [Export]
    public required PackedScene CardScene { get; set; }

    public Concentration Concentration { get; set; } = new();

    private List<CardNode> _cardNodes = new();

    public override void _Ready()
    {
        Concentration.CardAdded += OnCardAdded;
        Concentration.CardRemoved += OnCardRemoved;
        Concentration.FirstCardFlipped += OnFirstCardFlipped;
        Concentration.MatchAttempted += OnMatchAttempted;

        Concentration.Layout(
            new List<Suit>() { Suit.Clubs, Suit.Spades, Suit.Diamonds, Suit.Hearts }
            .SelectMany(s => Enumerable.Range(1, 10).Select(i =>
                new CardData { Rank = i, Suit = s, CardBack = GetCardColor(s), IsBomb = i == 1 }
            ))
            .ToList(),
            9
        );
    }

    public override void _ExitTree()
    {
        Concentration.CardAdded -= OnCardAdded;
        Concentration.CardRemoved -= OnCardRemoved;
        Concentration.FirstCardFlipped -= OnFirstCardFlipped;
        Concentration.MatchAttempted -= OnMatchAttempted;
    }

    public void OnCardAdded(Card card)
    {
        var cardNode = CardScene.Instantiate<CardNode>();
        cardNode.CardManager = this;
        cardNode.Card = card;
        AddChild(cardNode);
        _cardNodes.Add(cardNode);
    }

    public void OnCardRemoved(Card card)
    {
        _cardNodes.RemoveAll(c => c.Card == card);
    }

    public void OnFirstCardFlipped(Card card)
    {
        foreach (var cardNode in _cardNodes)
        {
            cardNode.SetGlow(
                cardNode.Card?.IsRevealed == true
                && cardNode.Card?.Data.Rank == card.Data.Rank
            );
        }
    }

    public void OnMatchAttempted(Card card)
    {
        foreach (var cardNode in _cardNodes)
        {
            cardNode.SetGlow(false);
        }
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
