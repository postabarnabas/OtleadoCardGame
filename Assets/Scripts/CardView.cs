using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public HandView parentHand;
    public Card card;
    public bool isSelected = false;
    public bool IsSelected => isSelected;
    public bool IsBeaten = false;
    public CardView BeatenBy = null;
    public bool isPickupSelectable = false; 

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager gm = FindObjectOfType<GameManager>();

        // 🔴 BEAT fázis
        if (gm.currentPhase == GameManager.TurnPhase.Beat)
        {
            // 🔥 ÜTÉSI CÉL KIVÁLASZTÁSA
            if (gm.IsSelectingBeatTarget(this))
            {
                gm.ResolveBeatSelection(this);
                return;
            }
            // 🟡 ASZTALI LAP → felvétel kijelölése
            if (parentHand == null && isPickupSelectable)
            {
                isSelected = !isSelected;
                UpdateOutline();
                return;
            }

            // 🟢 KÉZBEN LÉVŐ LAP → ütés
            if (parentHand != null && parentHand.IsActive)
            {
                gm.TryBeatWithCard(this);
                return;
            }

            return;
        }

        // 🔵 GIVE fázis
        if (gm.currentPhase == GameManager.TurnPhase.Give)
        {
            if (!parentHand.IsActive)
                return;

            isSelected = !isSelected;
            UpdateOutline();
        }
    }



    public void UpdateOutline()
    {
        image.color = isSelected ? Color.yellow : Color.white;
    }
    public void SetCard(Card c)
    {
        card = c;
        if (image == null)
            image = GetComponent<Image>();

        image.sprite = Resources.Load<Sprite>($"Cards/{c.GetCardFileName()}");
    }    
}
