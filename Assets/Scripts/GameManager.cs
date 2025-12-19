using System.Collections.Generic;
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
        Debug.Log("PLAY GOMB MEGNYOMVA");
        HandView currentHandView = handViews[currentPlayerIndex];
        Player currentPlayer = players[currentPlayerIndex];

        var selectedCards = currentHandView.GetSelectedCards();

        if (selectedCards.Count == 0)
        {
            Debug.Log("Nincs kiválasztott lap");
            return;
        }

        foreach (var cv in selectedCards)
        {
            // 1?? logikai eltávolítás
            currentPlayer.Hand.Remove(cv.card);

            // 2?? UI eltávolítás kézbõl
            currentHandView.RemoveCard(cv);

            // 3?? UI megjelenítés az asztalon
            GameObject tableCard = Instantiate(cardPrefab, tableArea);
            var tableCV = tableCard.GetComponent<CardView>();
            tableCV.SetCard(cv.card);
        }

        EndTurn();
    }
}
