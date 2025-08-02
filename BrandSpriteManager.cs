using Godot;
using System;

public partial class BrandSpriteManager : Node2D
{
    private Sprite2D? SuitSprite;
    private Sprite2D? RankSprite;
    private ShaderMaterial? BrandMaterial;
    private bool isBranded = false;

    public Suit suit;
    public int rank;

    public override void _Ready()
    {
        SuitSprite = GetNode<Sprite2D>("BrandSuitSprite");
        RankSprite = GetNode<Sprite2D>("BrandRankSprite");
        BrandMaterial = (ShaderMaterial)Material;

        SetBrandFrames(suit, rank);
    }

    public void RevealBrand(bool isFaceUp)
    {
        if (!isFaceUp && !isBranded)
        {
            var tween = CreateTween();
            tween.TweenMethod(
                Callable.From<float>(SetSpriteShaderParam),
                -0.2f,
                1.2f,
                1.5
            );
            isBranded = true;
        }
    }

    public void SetBrandFrames(Suit s, int r)
    {
        SuitSprite?.SetFrame((int)s + 3);
        RankSprite?.SetFrame(r + 6);
    }

    private void SetSpriteShaderParam(float value)
    {
        SuitSprite?.SetInstanceShaderParameter("noise_offset", value);
        RankSprite?.SetInstanceShaderParameter("noise_offset", value);
    }
}
