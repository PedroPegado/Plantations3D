using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    private SeedController seedController;

    private void Awake()
    {
        seedController = GetComponentInParent<SeedController>();
        if (seedController == null)
        {
            Debug.LogError("SeedPickup requer um SeedController no pai!", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCurrentSeedPickup(this);

                if (seedController != null && seedController.selectedSeed != null)
                {
                    string seedName = seedController.selectedSeed.seedName;
                    InteractionManager.Instance.Show($"Pressione 'E' para pegar {seedName}", transform.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ClearCurrentSeedPickup();
            }
            InteractionManager.Instance.Hide();
        }
    }
}