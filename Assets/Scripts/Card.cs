using UnityEngine;


public enum CardSuit
{
    Piros,
    Zold,
    Makk,
    Tok
}
public enum CardRank
{
    Hetes = 7,
    Nyolcas = 8,
    Kilences = 9,
    Tizes = 10,
    Also = 11,
    Felso = 12,
    Kiraly = 13,
    Asz = 14
}
public class Card
{
    public CardSuit Suit { get; private set; }
    public CardRank Rank { get; private set; }
    public Card (CardSuit suit, CardRank rank)
    {
        Suit = suit;
        Rank = rank;
    }
}
