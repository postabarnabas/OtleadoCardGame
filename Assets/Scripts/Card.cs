using UnityEngine;

public enum CardSuit
{
    Piros,
    Zold,
    Makk,
    Tok
}

public enum CardRank
{
    Hetes = 7,
    Nyolcas = 8,
    Kilences = 9,
    Tizes = 10,
    Also = 11,
    Felso = 12,
    Kiraly = 13,
    Asz = 14
}

public class Card
{
    public CardSuit Suit { get; private set; }
    public CardRank Rank { get; private set; }

    public Card(CardSuit suit, CardRank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public string GetCardFileName()
    {
        string suitName = Suit.ToString().ToLower();  
        string rankName = RankToString();            

        return $"{suitName}_{rankName}";
    }

    private string RankToString()
    {
        switch (Rank)
        {
            case CardRank.Hetes: return "hetes";
            case CardRank.Nyolcas: return "nyolcas";
            case CardRank.Kilences: return "kilences";
            case CardRank.Tizes: return "tizes";
            case CardRank.Also: return "also";   
            case CardRank.Felso: return "felso";
            case CardRank.Kiraly: return "kiraly";
            case CardRank.Asz: return "asz";
            default: return ((int)Rank).ToString();
        }
    }
}
