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
    private CardTrackerStack? cardTrackerStack;

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

        CardTrackerStack cardTrackerStack = new()
        {
            deck = deck.ToList()
        };

        cardTrackerStack.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(cardTrackerStack);
    }
}

public partial class CardTracker : Control
{
    public required Card Card { get; set; }
    private Label? label;

    public override void _Ready()
    {
        Card.Matched += SetMatched;
        label = new()
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

    public override void _ExitTree()
    {
        Card.Matched -= SetMatched;
    }

    public static Color GetColor(Suit suit)
    {
        return suit switch
        {
            //Placeholder
            Suit.Hearts => new Color(1.0f, 0.0f, 0.0f, 1.0f), // Red
            Suit.Diamonds => new Color(1.0f, 0.6f, 0.0f), // Orange
            Suit.Clubs => new Color(0.0f, 0.0f, 0.0f), // Black
            Suit.Spades => new Color(0.0f, 0.65f, 0.0f), // Dark Green
            _ => new Color(1.0f, 1.0f, 1.0f) // White
        };
    }

    public void SetMatched()
    {
        if (label == null) return;

        var tween = CreateTween();
        tween.TweenProperty(label, "modulate:a", 0.2f, 0.8f);
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
    public required CardBack CardBack; //Used for row position + icon
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

            if (rowCards.Count > 9)
            {
                CreateOverflowRow(back, rowCards);
            }
            else
            {
                row.Initialize(rowCards);
                AddChild(row);
            }
            var spacer = new Control();
            AddChild(spacer);
        }
    }

    private void CreateOverflowRow(CardBack cardBack, List<Card> cards)
    {
        int cardsPerLevel = 9;

        for (int i = 0; i < cards.Count; i += cardsPerLevel)
        {
            var levelCards = cards.Skip(i).Take(cardsPerLevel).ToList();
            var row = new CardTrackerRow { CardBack = cardBack };
            row.Initialize(levelCards);
            AddChild(row);
        }
    }
}