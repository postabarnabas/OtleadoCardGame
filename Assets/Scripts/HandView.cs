using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    public GameObject cardPrefab; // a CardPrefab_UI prefab
    private List<GameObject> spawned = new List<GameObject>();

    // Frissítjük a megjelenést a player hand alapján
    public void Refresh(Player player)
    {
        Clear();
        if (player == null) return;

        foreach (var card in player.Hand)
        {
            GameObject g = Instantiate(cardPrefab, transform);
            var cv = g.GetComponent<CardView>();
            if (cv != null)
            {
                // Itt alakítjuk a Card objektumot a Resources útvonal formátumára
                string resourcePath = $"Cards/{card.GetCardFileName()}";
                cv.SetCardImage(resourcePath);
            }
            spawned.Add(g);
        }
    }

    public void Clear()
    {
        foreach (var g in spawned)
            if (g != null) Destroy(g);
        spawned.Clear();
    }
}
