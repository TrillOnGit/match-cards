using Godot;
using System;

public partial class CardNode : Area2D
{
    [Export] public Sprite2D CardGlow { get; set; } = null!;
    [Export] public Sprite2D CardFaceSprite { get; set; } = null!;
    [Export] public Sprite2D CardBackSprite { get; set; } = null!;
    [Export] public Sprite2D BombSprite { get; set; } = null!;

    [Export] public CardBack CardBack { get; set; } = CardBack.Red;
    [Export] public int CardRank { get; set; } = 1;
    public Suit CardSuit { get; set; } = Suit.Clubs;
    public bool FaceUp { get; set; } = false;
    public bool Revealed { get; set; } = false;

    public Card? Card { get; set; } = null;
    public CardManager? CardManager { get; set; } = null;

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
        if (Card is { } card)
        {
            card.Flipped += Flip;
            card.Revealed += Reveal;
            card.Removed += QueueFree;

            CardRank = card.Data.Rank;
            CardBack = card.Data.CardBack;
            CardSuit = card.Data.Suit;
            BombSprite.Visible = card.Data.IsBomb;
            Position = new Vector2(card.X * 90f, card.Y * 128f);
        }

        CardBackSprite.Frame = GetBackFrame();
        CardFaceSprite.Texture = GetSuitSprite();
        CardFaceSprite.Frame = GetFaceFrame();
        UpdateSpriteVisibility();

        CardGlow.Visible = false;
    }

    public override void _ExitTree()
    {
        InputEvent -= OnInputEvent;
        if (Card != null)
        {
            Card.Flipped -= Flip;
            Card.Revealed -= Reveal;
            Card.Removed -= QueueFree;
        }
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton { } ev)
        {
            if (ev.ButtonIndex == MouseButton.Left && ev.Pressed && Card != null)
            {
                CardManager?.Concentration.Flip(Card);
            }
        }
    }

    public int GetBackFrame()
    {
        //When there's need for multiple cardbacks, this will be used to set the frame
        return CardBack switch
        {
            CardBack.Red => 0,
            CardBack.Blue => 1,
            _ => 0,
        };
    }

    public int GetFaceFrame()
    {
        return CardRank - 1;
    }

    public Texture2D? GetSuitSprite()
    {
        return CardSuit switch
        {
            Suit.Clubs => GD.Load("res://assets/SBS - 2D Poker Pack/Top-Down/Cards/Clubs-88x124.png") as Texture2D,
            Suit.Spades => GD.Load("res://assets/SBS - 2D Poker Pack/Top-Down/Cards/Spades-88x124.png") as Texture2D,
            Suit.Diamonds => GD.Load("res://assets/SBS - 2D Poker Pack/Top-Down/Cards/Diamonds-88x124.png") as Texture2D,
            Suit.Hearts => GD.Load("res://assets/SBS - 2D Poker Pack/Top-Down/Cards/Hearts-88x124.png") as Texture2D,
            _ => throw new NotImplementedException(),
        };
    }

    public void Flip(bool faceUp)
    {
        FaceUp = faceUp;
        UpdateSpriteVisibility();
    }
    public void Reveal()
    {
        Revealed = true;
        UpdateSpriteVisibility();
    }

    public void SetGlow(bool glow)
    {
        CardGlow.Visible = glow;
    }

    private void UpdateSpriteVisibility()
    {
        if (FaceUp)
        {
            CardBackSprite.Visible = false;
            CardFaceSprite.Visible = true;
            CardFaceSprite.Modulate = Color.FromHsv(0f, 0f, 1f, 1f);
        }
        else
        {
            CardBackSprite.Visible = true;
            if (Revealed)
            {
                CardFaceSprite.Visible = true;
                CardFaceSprite.Modulate = Color.FromHsv(0f, 0f, 1f, 0.8f);
            }
            else
            {
                CardFaceSprite.Visible = false;
            }
        }
    }
}

public enum CardBack
{
    Red,
    Blue,
}