using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(string name) : base(name, true)
    {

    }
    public List<Card> SelectCardsToGive()
    {
        List<Card> result = new List<Card>();

        Card weakest = GetWeakestCard();

        if (weakest != null)
        {
            result.Add(weakest);
        }

        return result;
    }

    private Card GetWeakestCard()
    {
        return Hand
            .OrderBy(c => (int)c.Rank)
            .FirstOrDefault();
    }
}
