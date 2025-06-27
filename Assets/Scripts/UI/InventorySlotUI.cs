using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public GameObject selectionBorder; 

    public void UpdateSlot(SeedData seed, int quantity)
    {
        if (seed != null)
        {
            iconImage.sprite = seed.icon;
            iconImage.enabled = true;
            quantityText.text = quantity.ToString();
            quantityText.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            quantityText.text = "";
            quantityText.enabled = false;
        }
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionBorder != null)
        {
            selectionBorder.SetActive(selected);
        }
    }
}