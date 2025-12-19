using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    public GameObject cardPrefab;
    private List<GameObject> spawned = new List<GameObject>();

    private bool isActive = false;
    public bool IsActive => isActive;

    public void SetActive(bool value)
    {
        isActive = value;
    }

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
                cv.parentHand = this;
                cv.SetCardImage($"Cards/{card.GetCardFileName()}");
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
