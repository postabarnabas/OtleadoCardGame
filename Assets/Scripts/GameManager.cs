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
}
