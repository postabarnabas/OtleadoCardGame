using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public string Name { get; private set; }
    public List<Card> Hand {  get; private set; }
    public bool IsAI { get; private set; }

    public Player(string name, bool isAI = false)
    {
        Name = name;
        IsAI = isAI;
        Hand = new List<Card>();
    }
    public void AddCard(Card card)
    {
        Hand.Add(card);
    }
    public void AddCards(List<Card> cards)
    {
        Hand.AddRange(cards);
    }
    public void RemoveCard(Card card)
    {
        Hand.Remove(card);
    }
    public void RemoveCards(List<Card> cards)
    {
        foreach (var card in cards)
        {
            Hand.Remove(card);
        }
    }
}
