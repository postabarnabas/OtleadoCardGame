using System.Collections.Generic;

public class AIBeatDecision
{
    public List<CardView> cards;      // Ütõ lapok
    public List<CardView> targets;    // Célpontok
    public bool pickup;

    // Felvétel
    public AIBeatDecision(bool pickup)
    {
        this.pickup = pickup;
        cards = new List<CardView>();
        targets = new List<CardView>();
    }

    // Egy lap ütése
    public AIBeatDecision(CardView card, CardView target)
    {
        this.pickup = false;
        cards = new List<CardView> { card };
        targets = new List<CardView> { target };
    }

    // Több lap ütése
    public AIBeatDecision(List<CardView> cards, List<CardView> targets)
    {
        this.pickup = false;
        this.cards = cards;
        this.targets = targets;
    }
}