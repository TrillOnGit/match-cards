using System.Collections.Generic;
using System.Linq;

interface IConcentration
{
    IEnumerable<Card> GetCards();
    Card? GetCardAtPos(int X, int Y);
    void Flip(Card card);
}

public class Concentration : IConcentration
{
    private readonly List<Card> _cards = new();
    // In concentration, you flip a card face up and then try another card.
    // This variable contains the first card, or null after you flip a pair.
    private Card? _lastFlipped;
    private int _points = 0;

    public void Flip(Card card)
    {
        card.Revealed = true;
        card.Flipped = true;
        if (_lastFlipped == null)
        {
            _lastFlipped = card;
            return;
        }
        MatchPair(_lastFlipped, card);
    }

    public Card? GetCardAtPos(int X, int Y) =>
        _cards.SingleOrDefault(c => c.X == X && c.Y == Y);

    public IEnumerable<Card> GetCards() => _cards;

    private void MatchPair(Card cardOne, Card cardTwo)
    {
        if (cardOne.Face.Rank == cardTwo.Face.Rank)
        {
            _points += 1;
        }
        else
        {
            cardOne.Flipped = false;
            cardTwo.Flipped = false;
        }
        _lastFlipped = null;
    }
}

public record Card
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Flipped { get; set; } = false;
    public bool Revealed { get; set; } = false;
    public required Face Face { get; init; }
}

public enum Suit
{
    Spades,
    Hearts,
    Clubs,
    Diamonds
}

public record Face
{
    public Suit Suit { get; set; }
    public int Rank { get; set; }
}