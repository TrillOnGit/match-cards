using System;
using System.Collections.Generic;
using MatchCards.Effects;

public class Run
{
    private List<CardData> _deck;

    public Concentration GenerateConcentration()
    {
        var concentration = new Concentration();
        concentration.Layout(_deck, 9);
        return concentration;
    }

    public CardChoice GenerateChoice() => new CardChoice(this);

    public void AddCard(CardData card)
    {
        _deck.Add(card);
    }
}

public class CardChoice
{
    private Run _run;

    private List<CardData> _choices;
    public IEnumerable<CardData> Choices => _choices;

    public CardChoice(Run run)
    {
        _run = run;
        _choices = GenerateChoices();
    }

    public void Choose(CardData card)
    {
        if (!_choices.Contains(card))
        {
            throw new ArgumentOutOfRangeException(nameof(card));
        }
        _run.AddCard(card);
    }

    private static List<CardData> GenerateChoices()
    {
        var rng = new Random();
        var choices = new List<CardData>();
        const int numChoices = 3;
        for (int i = 0; i < numChoices; i++)
        {
            choices.Add(GenerateRandomCard(rng));
        }
        return choices;
    }

    private static CardData GenerateRandomCard(Random rng)
    {
        var rank = rng.NextRank();
        var suit = rng.NextSuit();
        var cardBack = rng.NextCardBack();

        var stickers = new List<ICardSticker>();
        var stickerSelection = rng.Next(0, 10);
        if (stickerSelection == 0)
        {
            stickers.Add(new BombSticker());
        }
        else if (stickerSelection == 1)
        {
            stickers.Add(new LighterSticker());
        }
        else if (stickerSelection == 2)
        {
            stickers.Add(new StarSticker());
        }
        return new CardData()
        {
            Rank = rank,
            Suit = suit,
            CardBack = cardBack,
            Stickers = stickers
        };
    }
}