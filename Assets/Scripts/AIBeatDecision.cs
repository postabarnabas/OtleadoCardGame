public class AIBeatDecision
{
    public CardView card;
    public CardView target;
    public bool pickup;

    public AIBeatDecision(bool pickup)
    {
        this.pickup = pickup;
    }

    public AIBeatDecision(CardView card, CardView target)
    {
        this.card = card;
        this.target = target;
        this.pickup = false;
    }
}