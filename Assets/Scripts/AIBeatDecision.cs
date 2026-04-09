using System.Collections.Generic;
public class AIBeatDecision
{
    public List<CardView> cards;
    public List<CardView> targets;
    public bool pickup;
    public AIBeatDecision(bool pickup)
    {
        this.pickup = pickup;
        cards = new List<CardView>();
        targets = new List<CardView>();
    }
    public AIBeatDecision(CardView card, CardView target)
    {
        this.pickup = false;
        cards = new List<CardView> { card };
        targets = new List<CardView> { target };
    }
    public AIBeatDecision(List<CardView> cards, List<CardView> targets)
    {
        this.pickup = false;
        this.cards = cards;
        this.targets = targets;
    }
}