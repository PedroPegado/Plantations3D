using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem; 

public class HotbarUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform slotsParent;
    public GameObject inventorySlotPrefab;

    [Header("Game References")]
    public PlayerInventory playerInventory;

    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

    private int _selectedSlotIndex = -1;
    public int SelectedSlotIndex
    {
        get { return _selectedSlotIndex; }
        private set
        {
            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < uiSlots.Count)
            {
                uiSlots[_selectedSlotIndex].SetSelected(false);
            }

            _selectedSlotIndex = value;

            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < uiSlots.Count)
            {
                uiSlots[_selectedSlotIndex].SetSelected(true);
                SeedData currentSeed = GetSelectedSeed();
                if (currentSeed != null)
                {
                    Debug.Log($"Slot {(_selectedSlotIndex + 1)} selecionado: {currentSeed.seedName}");
                }
                else
                {
                    Debug.Log($"Slot {(_selectedSlotIndex + 1)} selecionado: Vazio");
                }
            }
            else
            {
                Debug.Log("Nenhum slot válido selecionado.");
            }

            onSelectedSeedChanged?.Invoke(GetSelectedSeed());
        }
    }

    public delegate void OnSelectedSeedChanged(SeedData selectedSeed);
    public event OnSelectedSeedChanged onSelectedSeedChanged;

    private InputSystem inputActions;

    private void Awake()
    {
        inputActions = new InputSystem();
        InitializeHotbarUI();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.onInventoryChangedCallback += UpdateUI;
        }


        inputActions.Hotbar.SelectSlot1.performed += ctx => SelectSlot(0);
        inputActions.Hotbar.SelectSlot2.performed += ctx => SelectSlot(1);
        inputActions.Hotbar.SelectSlot3.performed += ctx => SelectSlot(2);
        inputActions.Hotbar.SelectSlot4.performed += ctx => SelectSlot(3);
        inputActions.Hotbar.SelectSlot5.performed += ctx => SelectSlot(4);
        inputActions.Hotbar.SelectSlot6.performed += ctx => SelectSlot(5);
        inputActions.Hotbar.SelectSlot7.performed += ctx => SelectSlot(6);
        inputActions.Hotbar.SelectSlot8.performed += ctx => SelectSlot(7);

        inputActions.Hotbar.Enable();
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.onInventoryChangedCallback -= UpdateUI;
        }

        inputActions.Hotbar.SelectSlot1.performed -= ctx => SelectSlot(0);
        inputActions.Hotbar.SelectSlot2.performed -= ctx => SelectSlot(1);
        inputActions.Hotbar.SelectSlot3.performed -= ctx => SelectSlot(2);
        inputActions.Hotbar.SelectSlot4.performed -= ctx => SelectSlot(3);
        inputActions.Hotbar.SelectSlot5.performed -= ctx => SelectSlot(4);
        inputActions.Hotbar.SelectSlot6.performed -= ctx => SelectSlot(5);
        inputActions.Hotbar.SelectSlot7.performed -= ctx => SelectSlot(6);
        inputActions.Hotbar.SelectSlot8.performed -= ctx => SelectSlot(7);

        inputActions.Hotbar.Disable(); 
    }

    void Start()
    {
        UpdateUI();
        SelectSlot(0);
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
                uiSlots[i].UpdateSlot(null, 0);
            }
        }
        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < uiSlots.Count)
        {
            uiSlots[_selectedSlotIndex].SetSelected(true);
        }
        onSelectedSeedChanged?.Invoke(GetSelectedSeed());
    }

    private void SelectSlot(int index)
    {
        if (index >= 0 && index < playerInventory.maxInventorySlots)
        {
            SelectedSlotIndex = index; 
        }
        else
        {
            Debug.LogWarning($"Tentou selecionar um slot fora do alcance: {index}. Max slots: {playerInventory.maxInventorySlots}");
        }
    }

    public SeedData GetSelectedSeed()
    {
        if (SelectedSlotIndex >= 0 && SelectedSlotIndex < playerInventory.inventorySlots.Count)
        {
            return playerInventory.inventorySlots[SelectedSlotIndex]?.seed;
        }
        return null;
    }

    public SeedInventorySlot GetSelectedInventorySlot()
    {
        if (SelectedSlotIndex >= 0 && SelectedSlotIndex < playerInventory.inventorySlots.Count)
        {
            return playerInventory.inventorySlots[SelectedSlotIndex];
        }
        return null;
    }
}