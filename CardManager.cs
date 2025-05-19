using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardManager : Node
{
    [Export]
    public required PackedScene CardScene { get; set; }

    public Concentration Concentration { get; set; } = new();

    public override void _Ready()
    {
        Concentration.CardAdded += OnCardAdded;

        Concentration.Layout(
            new List<Suit>() { Suit.Clubs, Suit.Spades, Suit.Diamonds, Suit.Hearts }
            .SelectMany(s => Enumerable.Range(1, 10).Select(i =>
                new CardData { Rank = i, Suit = s, CardBack = GetCardColor(s) }
            ))
            .ToList()
        );
    }

    public override void _ExitTree()
    {
        Concentration.CardAdded -= OnCardAdded;
    }

    public void OnCardAdded(Card card)
    {
        var cardNode = CardScene.Instantiate<CardArea2d>();
        cardNode.CardManager = this;
        cardNode.Card = card;
        AddChild(cardNode);
    }

    public CardBack GetCardColor(Suit suit)
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
