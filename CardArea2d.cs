using Godot;
using System;

public partial class CardArea2d : Area2D
{
    [Export] public Sprite2D CardFaceSprite { get; set; } = null!;
    [Export] public Sprite2D CardBackSprite { get; set; } = null!;

    [Export] public CardBackColor CardBack { get; set; } = CardBackColor.Blue;
    [Export] public int CardFace { get; set; } = 1;
    public bool FaceUp { get; set; } = false;

    public override void _Ready()
    {
        CardBackSprite.Frame = GetBackFrame();
        CardFaceSprite.Frame = GetFaceFrame();
    }

    public int GetBackFrame()
    {
        //When there's need for multiple cardbacks, this will be used to set the frame
        return CardBack switch
        {
            CardBackColor.Red => 0,
            CardBackColor.Blue => 1,
            _ => 0,
        };
        ;
    }

    public int GetFaceFrame()
    {
        return CardFace - 1;
    }
}

public enum CardBackColor
{
    Red,
    Blue,
}