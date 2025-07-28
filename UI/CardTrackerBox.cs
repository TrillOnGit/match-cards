using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CardTrackerBox : Container
{
    // Whenever a card is added, this needs to add another element (read events). 
    // Those elements need to display text (A, 2) color coded. 
    // Those elements also need to detect mouse hovers. 
    // When the corresponding card is matched, we want to increase translucency
    public override void _Ready()
    {
        ScoreEventManager.CardsLaidOut += CreateDeckUI;
    }

    public override void _ExitTree()
    {
        ScoreEventManager.CardsLaidOut -= CreateDeckUI;
    }

    public void CreateDeckUI(IEnumerable<Card> deck)
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        CardTrackerStack stack = new()
        {
            deck = deck.ToList()
        };

        stack.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(stack);
    }
}

public partial class CardTracker : Control
{
    public required Card Card { get; set; }

    public override void _Ready()
    {
        Label label = new()
        {
            Text = GetText(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.Off
        };
        label.AddThemeColorOverride("font_color", GetColor(Card.Data.Suit));
        label.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(label);
    }

    public static Color GetColor(Suit suit)
    {
        return suit switch
        {
            //Placeholder
            Suit.Hearts => new Color(1, 0, 0), // Red
            Suit.Diamonds => new Color(0, 1, 0), // Green
            Suit.Clubs => new Color(0, 0, 1), // Blue
            Suit.Spades => new Color(0, 0, 0), // Black
            _ => new Color(1, 1, 1) // White
        };
    }

    public string GetText()
    {
        return Card.Data.Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => Card.Data.Rank.ToString()
        };
    }
}

public partial class CardTrackerRow : HBoxContainer
{
    public required CardBack CardBack; //Used for row icon
    private List<Card>? cards;

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 25);
    }

    public void Initialize(List<Card>? rowCards)
    {
        cards = rowCards;
        CreateCardTrackers();
    }

    private void CreateCardTrackers()
    {
        if (cards == null)
        {
            return;
        }

        foreach (Card card in cards)
        {
            CardTracker tracker = new() { Card = card };
            AddChild(tracker);
        }
    }
}

public partial class CardTrackerStack : VBoxContainer
{
    public List<Card>? deck;

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 20);

        if (deck == null || deck.Count == 0)
        {
            GD.PrintErr("Deck is not initialized or is empty.");
            return;
        }

        SetRows(deck);
    }

    public void SetRows(List<Card> deck)
    {
        foreach (CardBack back in Enum.GetValues(typeof(CardBack)))
        {
            var rowCards = deck.Where(card => card.Data.CardBack == back)
            .OrderBy(Card => (Card.Data.Rank, Card.Data.Suit))
            .ToList();
            var row = new CardTrackerRow { CardBack = back };
            row.Initialize(rowCards);
            AddChild(row);
        }
    }
}