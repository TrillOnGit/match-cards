using Godot;
using MatchCards.Effects;
using System;

public partial class CardNode : Area2D
{
    [Export] public Sprite2D CardGlow { get; set; } = null!;
    [Export] public Sprite2D CardFaceSprite { get; set; } = null!;
    [Export] public Sprite2D CardBackSprite { get; set; } = null!;
    [Export] public Sprite2D BombSprite { get; set; } = null!;
    [Export] public Sprite2D BurningSprite { get; set; } = null!;
    [Export] public Sprite2D LighterSprite { get; set; } = null!;
    [Export] public Sprite2D StarSprite { get; set; } = null!;
    [Export] public Sprite2D HunterSprite { get; set; } = null!;
    [Export] public Sprite2D KnowledgeSprite { get; set; } = null!;
    [Export] public Sprite2D LeftRevealSprite { get; set; } = null!;
    [Export] public Sprite2D RightRevealSprite { get; set; } = null!;
    [Export] public Sprite2D CorpseSprite { get; set; } = null!;
    [Export] public Sprite2D RitualSprite { get; set; } = null!;
    [Export] public Sprite2D EyeSprite { get; set; } = null!;
    [Export] public CpuParticles2D Sparkle { get; set; } = null!;
    [Export] public CardBack CardBack { get; set; } = CardBack.Red;
    [Export] public int CardRank { get; set; } = 1;
    public Suit CardSuit { get; set; } = Suit.Clubs;
    public bool FaceUp { get; set; } = false;
    public bool Revealed { get; set; } = false;
    public CardState State { get; set; } = CardState.Default;


    private CardData? _cardData = null;
    public CardData? CardData { get => Card?.Data ?? _cardData; set { _cardData = value; Card = null; } }
    public Card? Card { get; set; } = null;


    public event Action? Clicked = null;
    public event Action<CardNode>? Targeted = null;

    public override void _Ready()
    {
        InputEvent += OnInputEvent;
        if (Card is { } card)
        {
            card.Flipped += Flip;
            card.Revealed += Reveal;
            card.Hidden += Unreveal;
            card.Burned += Burn;
            card.Moved += Move;
            card.Removed += QueueFree;
            card.Matched += OnMatched;

            BurningSprite.Visible = card.IsBurning;
            UpdatePosition();
        }
        if (CardData is { } data)
        {
            CardRank = data.Rank;
            CardBack = data.CardBack;
            CardSuit = data.Suit;
            BombSprite.Visible = data.HasSticker<BombSticker>();
            LighterSprite.Visible = data.HasSticker<LighterSticker>();
            StarSprite.Visible = data.HasSticker<StarSticker>();
            HunterSprite.Visible = data.HasSticker<HunterSticker>();
            KnowledgeSprite.Visible = data.HasSticker<KnowledgeSticker>();
            LeftRevealSprite.Visible = data.HasSticker<LeftRevealSticker>();
            RightRevealSprite.Visible = data.HasSticker<RightRevealSticker>();
            CorpseSprite.Visible = data.HasSticker<CorpseSticker>();
            RitualSprite.Visible = data.HasSticker<RitualSticker>();
            EyeSprite.Visible = data.HasSticker<EyeSticker>();
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
            Card.Hidden -= Unreveal;
            Card.Moved -= Move;
            Card.Burned -= Burn;
            Card.Removed -= QueueFree;
            Card.Matched -= OnMatched;
        }
    }

    private void UpdatePosition()
    {
        if (Card == null) return;

        Position = new Vector2(Card.X * 90f, Card.Y * 128f);
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton { } ev)
        {
            if (ev.ButtonIndex == MouseButton.Left && ev.Pressed && State == CardState.Default)
            {
                OnClick();
            }
        }
    }

    private void OnClick()
    {
        // switch (State)
        // {
        //     case CardState.Default:
        Clicked?.Invoke();
        //break;
        //     //Remove these cases and this function when concentration handles game states
        //     case CardState.ActivationTarget:
        //         Card?.SendTargeted();
        //         break;
        //     default:
        //         break;
        // }
    }

    public int GetBackFrame()
    {
        //When there's need for multiple cardbacks, this will be used to set the frame
        return CardBack switch
        {
            CardBack.Red => 0,
            CardBack.Blue => 1,
            CardBack.Pink => 2,
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

    private void Flip(bool faceUp)
    {
        FaceUp = faceUp;
        UpdateSpriteVisibility();
    }
    private void Reveal()
    {
        Revealed = true;
        UpdateSpriteVisibility();
    }

    private void Burn()
    {
        BurningSprite.Visible = true;
    }
    private void Unreveal()
    {
        Revealed = false;
        UpdateSpriteVisibility();
    }

    private void Move(int x, int y)
    {
        UpdatePosition();
    }


    private void OnMatched()
    {
        Sparkle.Emitting = true;
    }

    public void SetGlow(CardGlowColor glow)
    {
        switch (glow)
        {
            case CardGlowColor.None:
                CardGlow.Visible = false;
                break;
            case CardGlowColor.Primary:
                CardGlow.Visible = true;
                CardGlow.Modulate = Color.FromHsv(3f / 6f, 1f, 1f); // cyan
                break;
            case CardGlowColor.Secondary:
                CardGlow.Visible = true;
                CardGlow.Modulate = Color.FromHsv(1f / 6f, 1f, 1f); // yellow
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(glow), $"Not a valid {nameof(CardGlowColor)}");
        }
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

public enum CardGlowColor
{
    None,
    Primary,
    Secondary
}

public enum CardState
{
    Default,
    ActivationTarget,
    ActivationNonTarget,
}