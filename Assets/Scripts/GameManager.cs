using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Player> players = new List<Player>();
    public GameObject cardPrefab; // ezt fogjuk példányosítani (a kártyakép)
    public Transform[] playerAreas; // ide rakjuk a játékos lapjait
    public int currentPlayerIndex = 0;
    public HandView[] handViews; // Player1 hand, Player2 hand
    public TMPro.TextMeshProUGUI currentPlayerText;
    public Transform tableArea;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        deck = new Deck();

        players.Add(new Player("Játékos 1"));
        players.Add(new Player("Játékos 2"));

        foreach (var player in players)
            player.AddCards(deck.DrawCards(5));

        for (int i = 0; i < players.Count; i++)
            handViews[i].Refresh(players[i]);

        currentPlayerIndex = 0;
        UpdateTurn();
    }
    void UpdateTurn()
    {
        for (int i = 0; i < handViews.Length; i++)
            handViews[i].SetActive(i == currentPlayerIndex);

        currentPlayerText.text =
            $"Aktuális játékos: Player {currentPlayerIndex + 1}";
    }
    void EndTurn()
    {
        handViews[currentPlayerIndex].SetActive(false);

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        handViews[currentPlayerIndex].SetActive(true);
        currentPlayerText.text = $"Aktuális játékos: Player {currentPlayerIndex + 1}";
    }
    public void OnPlayButtonClicked()
    {

        HandView currentHandView = handViews[currentPlayerIndex];
        Player currentPlayer = players[currentPlayerIndex];

        var selectedCardViews = currentHandView.GetSelectedCards();

        if (selectedCardViews.Count == 0)
        {
            Debug.Log("Nincs kiválasztott lap");
            return;
        }

        // 1️⃣ Card lista kinyerése
        List<Card> selectedCards = new List<Card>();
        foreach (var cv in selectedCardViews)
            selectedCards.Add(cv.card);

        // 2️⃣ Szabály ellenőrzés
        if (!IsValidSelection(selectedCards))
        {
            return;
        }

        // 3️⃣ Leadás (UI + logika)
        foreach (var cv in selectedCardViews)
        {
            // logikai eltávolítás
            currentPlayer.Hand.Remove(cv.card);

            // UI eltávolítás kézből
            currentHandView.RemoveCard(cv);

            // UI megjelenítés az asztalon
            GameObject tableCard = Instantiate(cardPrefab, tableArea);
            var tableCardView = tableCard.GetComponent<CardView>();
            tableCardView.SetCard(cv.card);
        }
        StartCoroutine(RefillAfterDelay(currentPlayer, currentHandView));

    }

    bool IsValidSelection(List<Card> cards)
    {
        int count = cards.Count;

        if (count == 1)
            return true;
        if (count == 2 || count == 4)
        {
            Debug.Log("1, 3 vagy 5 lapot lehet leadni.");
            return false;
        }
        if (count == 3)
        {
            return IsValidThree(cards);
        }
        if (count == 5)
        {
            return IsValidFive(cards);
        }
        return false;
    }
    bool IsValidThree(List<Card> cards)
    {
        CardRank r1 = cards[0].Rank;
        CardRank r2 = cards[1].Rank;
        CardRank r3 = cards[2].Rank;

        if (r1 == r2 && r2 == r3)
            return false; Debug.Log("Érvénytelen lapkombináció");

        if (r1 != r2 && r1 != r3 && r2 != r3)
            return false; Debug.Log("Érvénytelen lapkombináció");

        // egy pár és egy különböző
        return true;
    }
    public bool IsValidFive(List<Card> cards)
    {
        List<CardRank> distinctRanks = new List<CardRank>();
        foreach (var c in cards)
        {
            if (!distinctRanks.Contains(c.Rank)) distinctRanks.Add(c.Rank);
        }

        List<int> counts = new List<int>();
        foreach (var rank in distinctRanks)
        {
            int count = cards.Count(c => c.Rank == rank);
            counts.Add(count);
        }

        counts.Sort();

        if (counts.SequenceEqual(new List<int> { 1, 2, 2 })) return true;
        if (counts.SequenceEqual(new List<int> { 1, 4 })) return true;
        Debug.Log("Érvénytelen lapkombináció");
        return false;
        
    }

    void RefillHand(Player player, HandView handView)
    {
        while (player.Hand.Count < 5 && deck.Cards.Count > 0)
        {
            Card c = deck.DrawCard();
            player.Hand.Add(c);
        }
        handView.Refresh(player);
    }
    IEnumerator RefillAfterDelay(Player player, HandView handView)
    {
        yield return new WaitForSeconds(0.7f);
        RefillHand(player, handView);
        EndTurn();
    }
}
