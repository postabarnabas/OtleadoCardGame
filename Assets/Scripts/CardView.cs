using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public HandView parentHand;

    private bool isSelected = false;

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

    public void SetCardImage(string resourcePath)
    {
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
            image.sprite = sprite;
        else
            Debug.LogWarning("Nem található sprite: " + resourcePath);
    }
}
