using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(string name) : base(name, true)
    {

    }
    //leadáshoz
    #region
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

        moves = moves.GroupBy(m => string.Join(",",m.Select(c => $"{(int)c.Rank}_{(int)c.Suit}"))).Select(g => g.First()).ToList();
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
        var pairs = Hand.GroupBy(c=> c.Rank).Where(g=> g.Count() >=2).OrderBy(g =>g.Key).ToList();
        for (int i = 0; i < pairs.Count; i++)
        {
            for (int j = i; j < pairs.Count; j++)
            {
                var firstPair = pairs[i].Take(2).ToList();
                var secondPair = pairs[j].Take(2).ToList();
                if (i == j && pairs[i].Count() < 4)
                    continue;

                foreach (var extra in Hand.Where(c =>c.Rank != pairs[i].Key && c.Rank != pairs[j].Key))
                {
                    List<Card> moveCards = new List<Card>
                    {
                    firstPair[0],
                    firstPair[1],
                    secondPair[0],
                    secondPair[1],
                    extra
                    };
                    if (i == j)
                    {
                        var fourCards = pairs[i].ToList();
                        moveCards[0] =fourCards[0];
                        moveCards[1] =fourCards[1];
                        moveCards[2] =fourCards[2];
                        moveCards[3] = fourCards[3];
                    }
                    moves.Add(moveCards);
                }
            }
        }
    }
    private int EvaluateMove(List<Card> move, bool talonEmpty)
    {
        if (move.Count == Hand.Count && talonEmpty)
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
            int weakest = Hand.Min(c => (int)c.Rank);
            if (rank != weakest)
                score -= (rank - weakest) * 10;

            if (rank >= 12)
                score -= 120;

            if (rank == 14)
                score -= 150;
        }
        if (move.Count == 5)
            score += 350;
        else if (move.Count == 3)
            score += 250;
        else
            score += 40;

        // milyen lapokat ad le
        foreach (var card in move)
        {
            int rank =(int)card.Rank;
            if (rank<=9)
                score +=20;
            if (rank==10)
                score +=15;
            if (rank==11)
                score +=10;
            if (rank==12)
                score -=20;
            if (rank==13)
                score -=60;
            if (rank==14)
                score -=80;
        }

        //-------------------------------------------------
        // mi marad a kézben
        //-------------------------------------------------

        var groups = remaining.GroupBy(c => c.Rank);
        foreach (var g in groups)
        {
            if (g.Count() ==4)
                score += 70;
            if (g.Count() ==3)
                score += 60;
            if (g.Count() ==2)
            {
                int rank =(int)g.Key;
                if (rank==10)
                    score += 20;
                if (rank==11)
                    score += 40;
                if (rank==12)
                    score += 60;
                if (rank==13)
                    score += 90;
                if (rank==14)
                    score += 120;
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
                var extra = g.First(x=> x.Count()==1).First();
                int weakest = Hand.Min(c => (int)c.Rank);
                if ((int)extra.Rank !=weakest)
                    score -= 200;
            }
        }
        if (move.Count == 5)
        {
            var g = move.GroupBy(c => c.Rank).ToList();
            if (g.Any(x => x.Count() == 2))
            {
                var extra = g.First(x => x.Count() == 1).First();
                int weakest = Hand.Min(c => (int)c.Rank);
                if ((int)extra.Rank != weakest)
                    score -= 200;
            }
        }

        //-------------------------------------------------
        // talon üres -> agresszívebb játék
        //-------------------------------------------------

        if (talonEmpty)
        {
            if (move.Count == 5) score += 500;
            else if (move.Count == 3) score += 300;
            else score += 100;

            
        }
        if (Hand.Count == 2&&talonEmpty)
        {
            score = 0;
            foreach (var card in move)
            {
                int rank = (int)card.Rank;
                switch (rank)
                {
                    case 7: score += 0; break;
                    case 8: score += 5; break;
                    case 9: score += 10; break;
                    case 10: score += 20; break;
                    case 11: score += 40; break;
                    case 12: score += 60; break;
                    case 13: score += 80; break;
                    case 14: score += 100; break;
                    default: score += 0; break;
                }
            }
        }
        /*
        Debug.Log(
            string.Join(",", move.Select(c => c.Rank)) +
            " -> " + score
        );*/

        return score;
    }
    #endregion
    //ütéshez
    #region
    public AIBeatDecision DecideBeat(List<CardView> targets, HandView handView, GameManager gm)
    {
        bool talonEmpty = gm.deck.Cards.Count == 0;
        List<Card> currentHand = handView.GetCards();
        int handSize = currentHand.Count;
        var attackersInHand = handView.GetComponentsInChildren<CardView>();

        if (handSize > 7 && !talonEmpty)
        {
            List<CardView> selectedAttackers = new List<CardView>();
            List<CardView> selectedTargets = new List<CardView>();
            List<Card> handCopy = new List<Card>(currentHand);

            // Végigmegyünk minden asztali lapon
            foreach (var target in targets)
            {
                // Megkeressük a leggyengébb ütő lapot ehhez a célhoz
                CardView bestForThisTarget = null;
                int weakestRank = int.MaxValue;

                foreach (var attacker in attackersInHand)
                {
                    if (attacker != null && handCopy.Contains(attacker.card) && gm.CanBeat(attacker.card, target.card))
                    {
                        int rank = (int)attacker.card.Rank;
                        if (rank < weakestRank)
                        {
                            weakestRank = rank;
                            bestForThisTarget = attacker;
                        }
                    }
                }
                // Ha találtunk, felhasználjuk
                if (bestForThisTarget != null)
                {
                    selectedAttackers.Add(bestForThisTarget);
                    selectedTargets.Add(target);
                    handCopy.Remove(bestForThisTarget.card);
                }
            }
            // Ha sikerült legalább egy lapot ütni, visszatérünk a több lapos ütéssel
            if (selectedAttackers.Count > 0)
            {
                Debug.Log($"[AI] SOK LAP ({handSize}) -> {selectedAttackers.Count} lap ütése");
                return new AIBeatDecision(selectedAttackers, selectedTargets);
            }
        }

        CardView bestCard = null;
        CardView bestTarget = null;
        int bestScore = int.MinValue;

        // Végigmegyünk minden célponton és minden ütő lapon
        foreach (var target in targets)
        {
            foreach (var attacker in attackersInHand)
            {
                if (attacker != null && gm.CanBeat(attacker.card, target.card))
                { 
                    List<Card> remaining = new List<Card>(currentHand);
                    remaining.Remove(attacker.card);
                    int score;
                    if (talonEmpty)
                    {
                        // Ha üres a talon, a gyengébb lapokat részesítjük előnyben
                        score = 1000 - (int)attacker.card.Rank;
                    }
                    else
                    {
                        // Egyébként a stratégiai pontozás
                        score = EvaluateBeat(attacker.card, target.card, remaining);
                    }

                    Debug.Log($"[ÜTÉS] {attacker.card.Suit} {attacker.card.Rank} -> {target.card.Suit} {target.card.Rank} = {score} pont");

                    // Ha jobb, mint az eddigi legjobb, eltároljuk
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestCard = attacker;
                        bestTarget = target;
                    }
                }
            }
        }

        int pickupScore = EvaluatePickup(targets, currentHand, talonEmpty);

        Debug.Log($"[DÖNTÉS] Legjobb ütés: {bestCard?.card} = {bestScore} pont");
        Debug.Log($"[DÖNTÉS] Felvétel pontszáma: {pickupScore}");

        if (bestCard == null)
        {
            Debug.Log("[DÖNTÉS] Nincs ütési lehetőség -> FELVÉTEL");
            return new AIBeatDecision(true);
        }

        // Ha a felvétel jobb, mint a legjobb ütés -> felvétel
        if (!talonEmpty && pickupScore > bestScore)
        {
            Debug.Log("[DÖNTÉS] Felvétel jobb -> FELVÉTEL");
            return new AIBeatDecision(true);
        }

        Debug.Log($"[DÖNTÉS] Ütés választva: {bestCard.card} -> {bestTarget.card}");
        return new AIBeatDecision(bestCard, bestTarget);
    }
    int EvaluateBeat(Card attacker, Card target, List<Card> remainingAfterBeat)
    {
        int score = 0;
        int attackerRank = (int)attacker.Rank;
        int targetRank = (int)target.Rank;

        // Alap pontozás: minél kisebb a különbség, annál jobb
        // (tehát a leggyengébb ütő lapot használja)
        score -= (attackerRank - targetRank) * 5;

        // Erős lapokkal ütés büntetése
        if (attackerRank == 12) score -= 20;
        if (attackerRank == 13) score -= 30;
        if (attackerRank == 14) score -= 40;

        // Gyenge lapokkal ütés jutalmazása
        if (attackerRank <= 11) score += 50;

        // Erős célpont ütése jutalmazása
        if (targetRank >= 11) score += 60;
        if (targetRank == 13) score += 80;

        // Ha a maradék kézben van pár, plusz pont
        var groups = remainingAfterBeat.GroupBy(c => c.Rank);
        foreach (var g in groups)
        {
            if (g.Count() >= 2) score += 75;
        }
        return score;
    }
    int EvaluatePickup(List<CardView> targets, List<Card> currentHand, bool talonEmpty)
    {
        int score = 0;
        foreach (var t in targets)
        {
            int rank = (int)t.card.Rank;
            // Gyenge lap felvétele kisebb büntetés
            if (rank <= 9) score -= 30;

            // Erős lap felvétele nagyobb büntetés (mert az ellenfélnek adtuk)
            if (rank >= 11) score -= 10;
            if (rank >= 13) score -= 5;

            // Megnézzük, lehet-e belőle később kombináció
            var wouldHave = new List<Card>(currentHand) { t.card };
            var groups = wouldHave.GroupBy(c => c.Rank);

            foreach (var g in groups)
            {
                if (g.Count() == 2) score += 25;
                if (g.Count() == 3) score += 30;
                if (g.Count() == 4) score += 35;
            }
        }
        if (talonEmpty) score = -50;
        return score;
    }
    #endregion
}
