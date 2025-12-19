using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public HandView parentHand;
    public Card card;
    private bool isSelected = false;
    public bool IsSelected => isSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentHand == null) return;
        if (!parentHand.IsActive) return;

        isSelected = !isSelected;
        UpdateOutline();
    }

    void UpdateOutline()
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
