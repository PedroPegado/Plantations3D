using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public List<SeedInventorySlot> inventorySlots = new List<SeedInventorySlot>();

    [Tooltip("N�mero m�ximo de slots diferentes no invent�rio de sementes")]
    public int maxInventorySlots = 8;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChangedCallback;

    public bool AddSeed(SeedData seedToAdd, int amount = 1)
    {
        if (seedToAdd == null)
        {
            Debug.LogWarning("Tentou adicionar uma semente nula.");
            return false;
        }

        foreach (SeedInventorySlot slot in inventorySlots)
        {
            if (slot.seed == seedToAdd)
            {
                int maxStackSize = 99;
                int spaceLeft = maxStackSize - slot.quantity;
                int amountToStack = Mathf.Min(amount, spaceLeft); 

                slot.quantity += amountToStack;
                amount -= amountToStack; 

                if (amountToStack > 0) 
                {
                    if (ItemNotificationManager.Instance != null)
                    {
                        ItemNotificationManager.Instance.AddNotification(seedToAdd.icon, seedToAdd.seedName, amountToStack);
                    }

                    Debug.Log($"Adicionou {seedToAdd.seedName} x{amountToStack} � pilha existente. Total: {slot.quantity}");
                    onInventoryChangedCallback?.Invoke();
                }

                if (amount <= 0) 
                {
                    return true;
                }
            }
        }

        if (inventorySlots.Count < maxInventorySlots && amount > 0)
        {
            int maxStackSize = 99;
            while (amount > 0)
            {
                if (inventorySlots.Count >= maxInventorySlots)
                {
                    Debug.LogWarning($"Invent�rio de sementes cheio! N�o foi poss�vel adicionar todas as {seedToAdd.seedName}. Restante: {amount}");
                    onInventoryChangedCallback?.Invoke();
                    return false; 
                }

                int amountToAdd = Mathf.Min(amount, maxStackSize);
                inventorySlots.Add(new SeedInventorySlot(seedToAdd, amountToAdd));
                amount -= amountToAdd; 


                if (ItemNotificationManager.Instance != null)
                {
                    ItemNotificationManager.Instance.AddNotification(seedToAdd.icon, seedToAdd.seedName, amountToAdd);
                }

                Debug.Log($"Adicionou {seedToAdd.seedName} x{amountToAdd} em um novo slot.");
            }
            onInventoryChangedCallback?.Invoke();
            return true;
        }
        else if (amount > 0) 
        {
            Debug.LogWarning($"Invent�rio de sementes cheio! N�o foi poss�vel adicionar {seedToAdd.seedName}.");
            return false;
        }

        onInventoryChangedCallback?.Invoke();
        return true; 
    }

    public bool RemoveSeed(SeedData seedToRemove, int amount = 1)
    {
        if (seedToRemove == null)
        {
            Debug.LogWarning("Tentou remover uma semente nula.");
            return false;
        }

        for (int i = inventorySlots.Count - 1; i >= 0; i--)
        {
            SeedInventorySlot slot = inventorySlots[i];
            if (slot.seed == seedToRemove)
            {
                if (slot.quantity > amount)
                {
                    slot.quantity -= amount;
                    Debug.Log($"Removeu {amount} de {seedToRemove.seedName}. Restante: {slot.quantity}");
                    amount = 0;
                    break;
                }
                else
                {
                    amount -= slot.quantity;
                    Debug.Log($"Removeu todas as {slot.quantity} unidades de {seedToRemove.seedName} de um slot.");
                    inventorySlots.RemoveAt(i);
                }
            }
        }

        if (amount > 0)
        {
            Debug.LogWarning($"N�o foi poss�vel remover todas as {seedToRemove.seedName}. {amount} restante.");
            onInventoryChangedCallback?.Invoke();
            return false;
        }

        onInventoryChangedCallback?.Invoke();
        return true;
    }

    public int GetSeedQuantity(SeedData seed)
    {
        int total = 0;
        foreach (var slot in inventorySlots)
        {
            if (slot.seed == seed)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    public void ClearInventory()
    {
        inventorySlots.Clear();
        onInventoryChangedCallback?.Invoke();
        Debug.Log("Invent�rio de sementes limpo.");
    }
}