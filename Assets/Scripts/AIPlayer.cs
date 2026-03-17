using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(string name) : base(name, true)
    {

    }
    public List<Card> SelectCardsToGive(int opponentCardCount, bool talonEmpty)
    {
        var moves = GenerateAllMoves(opponentCardCount, talonEmpty);
        List<Card> bestMove = null;
        int bestScore = int.MinValue;
        foreach (var move in moves)
        {
            int score = EvaluateMove(move, talonEmpty);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }
    private List<List<Card>> GenerateAllMoves(int opponentCardCount, bool talonEmpty)
    {
        List<List<Card>> moves = new List<List<Card>>();

        int maxLead = 5;

        if (talonEmpty)
            maxLead = opponentCardCount;

        AddSingleCardMoves(moves);

        if (maxLead >= 3)
            AddThreeCardMoves(moves);

        if (maxLead >= 5)
            AddFiveCardMoves(moves);

        moves = moves.GroupBy(m => string.Join(",", m.Select(c => c.Rank))).Select(g => g.First()).ToList();
        return moves;
    }
    private void AddSingleCardMoves(List<List<Card>> moves)
    {
        foreach (var card in Hand)
        {
            moves.Add(new List<Card>(new List<Card> { card }));
        }
    }
    private void AddThreeCardMoves(List<List<Card>> moves)
    {
        var groups = Hand.GroupBy(c => c.Rank);

        foreach (var pair in groups.Where(g => g.Count() >= 2))
        {
            var pairCards = pair.Take(2).ToList();

            foreach (var extra in Hand.Where(c => c.Rank != pair.Key))
            {
                List<Card> moveCards = new List<Card>
                {
                pairCards[0],
                pairCards[1],
                extra
                };

                moves.Add(new List<Card>(moveCards));
            }
        }
    }
    private void AddFiveCardMoves(List<List<Card>> moves)
    {
        var pairs = Hand
            .GroupBy(c => c.Rank)
            .Where(g => g.Count() >= 2)
            .OrderBy(g => g.Key)  // rendezés a konzisztenciáért
            .ToList();

        // Minden lehetséges pár kombináció
        for (int i = 0; i < pairs.Count; i++)
        {
            for (int j = i; j < pairs.Count; j++)  // j = i-től indul!
            {
                // Első pár lapjai (2 vagy 4)
                var firstPair = pairs[i].Take(2).ToList();

                // Második pár lapjai (2 vagy 4)
                var secondPair = pairs[j].Take(2).ToList();

                // Ha ugyanaz a pár, és nincs legalább 4 lap belőle, skip
                if (i == j && pairs[i].Count() < 4)
                    continue;

                // Kísérő: olyan lap, ami nem a két pár rangjából való
                foreach (var extra in Hand.Where(c =>
                    c.Rank != pairs[i].Key && c.Rank != pairs[j].Key))
                {
                    List<Card> moveCards = new List<Card>
                    {
                    firstPair[0],
                    firstPair[1],
                    secondPair[0],
                    secondPair[1],
                    extra
                    };

                    // Ha ugyanaz a pár volt, a 4 lapból 2-2-t veszünk
                    if (i == j)
                    {
                        // Négy egyforma esetén a 4 lapból választunk 2-t
                        var fourCards = pairs[i].ToList();
                        moveCards[0] = fourCards[0];
                        moveCards[1] = fourCards[1];
                        moveCards[2] = fourCards[2];
                        moveCards[3] = fourCards[3];
                    }

                    moves.Add(moveCards);
                }
            }
        }
    }
    private int EvaluateMove(List<Card> move, bool talonEmpty)
    {
        // ha az AI le tudja adni az összes lapját -> nyer
        if (move.Count == Hand.Count)
            return 10000;

        int score = 0;

        List<Card> remaining = new List<Card>(Hand);

        foreach (var card in move)
            remaining.Remove(card);

        //-------------------------------------------------
        // mennyi lapot ad le
        //-------------------------------------------------
        // --------------------------------------------------
        // erős lap egyedül leadása nagyon rossz
        // --------------------------------------------------

        if (move.Count == 1)
        {
            var card = move[0];
            int rank = (int)card.Rank;

            if (rank >= 11) // felső vagy erősebb
                score -= 120;

            if (rank >= 13) // ász
                score -= 150;
        }

        if (move.Count == 5)
            score += 500;

        else if (move.Count == 3)
            score += 250;

        else
            score += 10;

        //-------------------------------------------------
        // milyen lapokat ad le
        //-------------------------------------------------

        foreach (var card in move)
        {
            int rank = (int)card.Rank;

            // gyenge lap leadása jó
            if (rank <= 9)
                score += 15;

            // 10 leadása sem rossz
            if (rank == 10)
                score += 5;

            // felső
            if (rank == 11)
                score -= 20;

            // király
            if (rank == 12)
                score -= 40;

            // ász
            if (rank == 13)
                score -= 80;
        }

        //-------------------------------------------------
        // mi marad a kézben
        //-------------------------------------------------

        var groups = remaining.GroupBy(c => c.Rank);

        foreach (var g in groups)
        {
            if (g.Count() == 4)
                score += 150;

            if (g.Count() == 3)
                score += 100;

            if (g.Count() == 2)
            {
                int rank = (int)g.Key;

                if (rank <= 9)
                    score += 30;

                if (rank == 10)
                    score += 50;

                if (rank == 11)
                    score += 80;

                if (rank == 12)
                    score += 120;

                if (rank == 13)
                    score += 160;
            }
        }

        //-------------------------------------------------
        // gyenge lap maradása rossz
        //-------------------------------------------------

        foreach (var card in remaining)
        {
            int rank = (int)card.Rank;

            if (rank <= 9)
                score -= 10;
        }

        //-------------------------------------------------
        // kísérő választás (pár + extra)
        //-------------------------------------------------

        if (move.Count == 3)
        {
            var g = move.GroupBy(c => c.Rank).ToList();

            if (g.Any(x => x.Count() == 2))
            {
                var extra = g.First(x => x.Count() == 1).First();

                int weakest = Hand.Min(c => (int)c.Rank);

                // ha nem a leggyengébb lap a kísérő → büntetés
                if ((int)extra.Rank != weakest)
                {
                    score -= 200;
                }
            }
        }

        //-------------------------------------------------
        // talon üres -> agresszívebb játék
        //-------------------------------------------------

        if (talonEmpty)
        {
            score += move.Count * 20;
        }

        //-------------------------------------------------
        // ha 2 lap marad -> erősebb maradjon
        //-------------------------------------------------

        if (remaining.Count == 2)
        {
            int weakest = remaining.Min(c => (int)c.Rank);
            score -= weakest * 2;
        }
        // --------------------------------------------------
        // ha egy lapot ad le, legyen a leggyengébb
        // --------------------------------------------------

        if (move.Count == 1)
        {
            int weakest = Hand.Min(c => (int)c.Rank);
            int played = (int)move[0].Rank;

            score -= (played - weakest) * 10;
        }
        
        Debug.Log(
            string.Join(",", move.Select(c => c.Rank)) +
            " -> " + score
        );

        return score;
    }
    public AIBeatDecision DecideBeat(List<CardView> targets, HandView handView, GameManager gm)
    {
        CardView bestCard = null;
        int bestScore = int.MinValue;

        bool talonEmpty = gm.deck.Cards.Count == 0;

        foreach (var target in targets)
        {
            foreach (Transform t in handView.transform)
            {
                CardView attacker = t.GetComponent<CardView>();

                if (attacker != null && gm.CanBeat(attacker.card, target.card))
                {
                    int score;

                    if (talonEmpty)
                    {
                        score = 1000 - (int)attacker.card.Rank;
                    }
                    else
                    {
                        score = EvaluateBeat(attacker.card, target.card);
                    }
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestCard = attacker;
                    }
                }
            }
        }

        // ha nincs ütőlap, vagy a felvétel jobb
        int pickupScore = EvaluatePickup(targets);
        if (bestCard == null || (!talonEmpty && pickupScore > bestScore))
        {
            return new AIBeatDecision(true);
        }

        return new AIBeatDecision(bestCard, targets.First());
    }
    int EvaluateBeat(Card attacker, Card target)
    {
        int score = 0;
        int attackerRank = (int)attacker.Rank;
        int targetRank = (int)target.Rank;
        score -= (attackerRank - targetRank) * 5;
        if (attackerRank >= 12)
            score -= 40;
        if (attackerRank == 13)
            score -= 60;
        if (attackerRank <= 9)
            score += 20;
        if (targetRank >= 11)
            score += 25;
        return score;
    }
    int EvaluatePickup(List<CardView> targets)
    {
        int score = 0;
        foreach (var t in targets)
        {
            int rank = (int)t.card.Rank;
            if (rank >= 11)
                score -= 5;
            if (rank <= 9)
                score -= 30;
        }
        return score;
    }
}
