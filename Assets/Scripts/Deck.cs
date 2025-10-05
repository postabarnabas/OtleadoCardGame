using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = System.Random;
using System.Linq;

public class Deck
{
    public List<Card> Cards { get; private set; }
    public Deck()
    {
        Cards = new List<Card>();
        CreateDeck();
        Shuffle();
    }
    private void CreateDeck()
    {
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }
    public void Shuffle()
    {
        Random rng = new Random();
        Cards=Cards.OrderBy(card=>rng.Next()).ToList();
    }
    public Card DrawCard()
    {
        if (Cards.Count == 0) return null;
        Card topCard = Cards[0];
        Cards.RemoveAt(0);
        return topCard;
    }
    public List<Card> DrawCards(int count)
    {
        List<Card> drawnCards = new List<Card>();
        for (int i = 0; i<count; i++) 
        { 
            drawnCards.Add(DrawCard());
        }
        Cards.RemoveRange(0, count);
        return drawnCards;
    }
}
