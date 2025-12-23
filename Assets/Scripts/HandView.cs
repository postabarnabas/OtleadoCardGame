using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    public GameObject cardPrefab;
    private List<CardView> spawned = new List<CardView>();

    private bool isActive = false;
    public bool IsActive => isActive;

    public void SetActive(bool value)
    {
        isActive = value;
    }

    public void Refresh(Player player)
    {
        if (player == null)
            return;

        Clear();

        foreach (var card in player.Hand)
        {
            GameObject g = Instantiate(cardPrefab, transform);

            var cv = g.GetComponent<CardView>();

            if (cv != null)
            {
                cv.parentHand = this;
                cv.SetCard(card);
            }

            spawned.Add(cv);
        }
    }

    public List<CardView> GetSelectedCards()
    {
        List<CardView> selected = new List<CardView>();

        foreach (var cv in spawned)
        {
            if (cv.IsSelected)
                selected.Add(cv);
        }

        return selected;
    }
    public void RemoveCard(CardView cv)
    {
        spawned.Remove(cv);
        Destroy(cv.gameObject);
    }
    public void Clear()
    {

        foreach (var g in spawned)
        {
            if (g != null)
            {
                Destroy(g.gameObject);
            }
        }
        spawned.Clear();
    }

}
