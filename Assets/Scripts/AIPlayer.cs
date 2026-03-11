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
        List<Card> result = FindFiveCardCombination();

        if (result != null)
            return result;

        result = FindThreeCardCombination();

        if (result != null)
            return result;

        Card weakest = GetWeakestCard();

        if (weakest != null)
            return new List<Card> { weakest };

        return new List<Card>();
    }

    private Card GetWeakestCard()
    {
        return Hand.OrderBy(c => (int)c.Rank).FirstOrDefault();
    }
    private List<Card> FindThreeCardCombination()
    {
        var groups = Hand.GroupBy(c => c.Rank);

        foreach (var group in groups)
        {
            if (group.Count() >= 2)
            {
                Card first = group.ElementAt(0);
                Card second = group.ElementAt(1);

                Card third = Hand
                    .Where(c => c.Rank != group.Key)
                    .OrderBy(c => (int)c.Rank)
                    .FirstOrDefault();

                if (third != null)
                {
                    return new List<Card> { first, second, third };
                }
            }
        }

        return null;
    }
    private List<Card> FindFiveCardCombination()
    {
        var groups = Hand
            .GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .ToList();

        if (groups.Count < 2)
            return null;

        for (int i = 0; i < groups.Count; i++)
        {
            for (int j = i + 1; j < groups.Count; j++)
            {
                Card first1 = groups[i].ElementAt(0);
                Card first2 = groups[i].ElementAt(1);

                Card second1 = groups[j].ElementAt(0);
                Card second2 = groups[j].ElementAt(1);

                Card extra = Hand
                    .Where(c => c.Rank != groups[i].Key && c.Rank != groups[j].Key)
                    .OrderBy(c => (int)c.Rank)
                    .FirstOrDefault();

                if (extra != null)
                {
                    return new List<Card>
                {
                    first1,
                    first2,
                    second1,
                    second2,
                    extra
                };
                }
            }
        }

        return null;
    }
}
