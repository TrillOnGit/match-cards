using Godot;
using System;

public partial class CardArea2d : Area2D
{
    [Export] public Sprite2D CardFaceSprite { get; set; } = null!;
    [Export] public Sprite2D CardBackSprite { get; set; } = null!;

    [Export] public CardBack CardBack { get; set; }
    [Export] public int CardFace { get; set; } = 1;
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

            CardFace = card.Data.Rank;
            CardBack = card.Data.CardBack;
            Position = new Vector2(card.X * 100f, card.Y * 100f);
        }

        CardBackSprite.Frame = GetBackFrame();
        CardFaceSprite.Frame = GetFaceFrame();
        UpdateSpriteVisibility();
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
        ;
    }

    public int GetFaceFrame()
    {
        return CardFace - 1;
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