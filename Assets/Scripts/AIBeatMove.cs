using System.Collections.Generic;

public class AIBeatMove
{
    public List<Card> attackers;  // Ütõ lapok (1 vagy több)
    public List<Card> targets;    // Célpontok (1 vagy több)
    public bool isPickup;          // Felvétel?

    public AIBeatMove(bool pickup)
    {
        isPickup = pickup;
        attackers = new List<Card>();
        targets = new List<Card>();
    }

    public AIBeatMove(Card attacker, Card target)
    {
        isPickup = false;
        attackers = new List<Card> { attacker };
        targets = new List<Card> { target };
    }

    public AIBeatMove(List<Card> attackers, List<Card> targets)
    {
        isPickup = false;
        this.attackers = attackers;
        this.targets = targets;
    }
}