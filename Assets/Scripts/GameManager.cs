using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Player> players = new List<Player>();
    public GameObject cardPrefab; // ezt fogjuk példányosítani (a kártyakép)
    public Transform[] playerAreas; // ide rakjuk a játékos lapjait

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        deck = new Deck();

        // két játékos példaként
        players.Add(new Player("Játékos 1"));
        players.Add(new Player("Játékos 2"));

        // mindenkinek 5 lap
        foreach (var player in players)
        {
            player.AddCards(deck.DrawCards(5));
        }

        // lapok megjelenítése
        DisplayHands();
    }

    void DisplayHands()
    {
        for (int i = 0; i < players.Count; i++)
        {
            foreach (Card card in players[i].Hand)
            {
                // Kártya prefab létrehozása a megfelelõ játékos zónában
                GameObject cardObj = Instantiate(cardPrefab, playerAreas[i]);

                // CardView komponens beállítása
                var cardView = cardObj.GetComponent<CardView>();
                if (cardView != null)
                {
                    // A CardView-n keresztül állítjuk be a sprite-ot
                    cardView.SetCardImage($"Cards/{card.GetCardFileName()}");
                }
            }
        }
    }
}
