using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    public SeedData seedData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddSeed(seedData);
                Destroy(gameObject);
            }
        }
    }
}
