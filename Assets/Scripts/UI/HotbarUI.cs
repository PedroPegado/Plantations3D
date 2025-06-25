using UnityEngine;
using System.Collections.Generic;
using TMPro; 

public class HotbarUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform slotsParent;     
    public GameObject inventorySlotPrefab; 

    [Header("Game References")]
    public PlayerInventory playerInventory; 

    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

    private void Awake()
    {
        InitializeHotbarUI();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.onInventoryChangedCallback += UpdateUI;
        }
        else
        {
            Debug.LogError("PlayerInventory não atribuído ao HotbarUI no Inspector!", this);
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.onInventoryChangedCallback -= UpdateUI;
        }
    }

    void Start()
    {
        UpdateUI();
    }

    private void InitializeHotbarUI()
    {
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        for (int i = 0; i < playerInventory.maxInventorySlots; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, slotsParent);
            InventorySlotUI uiSlot = slotObj.GetComponent<InventorySlotUI>();
            if (uiSlot != null)
            {
                uiSlots.Add(uiSlot);
                uiSlot.UpdateSlot(null, 0);
            }
            else
            {
                Debug.LogError("Prefab do slot do inventário não tem o script InventorySlotUI!", inventorySlotPrefab);
            }
        }
        Debug.Log($"Criados {uiSlots.Count} slots de UI para a hotbar.");
    }

    public void UpdateUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            if (i < playerInventory.inventorySlots.Count)
            {
                SeedInventorySlot inventorySlot = playerInventory.inventorySlots[i];
                uiSlots[i].UpdateSlot(inventorySlot.seed, inventorySlot.quantity);
            }
            else
            {
                // Slot vazio
                uiSlots[i].UpdateSlot(null, 0);
            }
        }
    }
}