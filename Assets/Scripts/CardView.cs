using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Image image;
    public bool isSelected = false;

public void OnClick()
{
    isSelected = !isSelected;
    UpdateOutline();
}

void UpdateOutline()
{
    // UI megvalósítás: pl. színváltás vagy outline
    GetComponent<Image>().color = isSelected ? Color.yellow : Color.white;
    public void SetCardImage(string resourcePath)
    {
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
            image.sprite = sprite;
        else
            Debug.LogWarning("Nem található sprite: " + resourcePath);
    }
}
