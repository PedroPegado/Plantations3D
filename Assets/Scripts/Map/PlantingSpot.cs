using UnityEngine;
using System.Collections; 

public class PlantingSpot : MonoBehaviour
{
    public bool isOccupied { get; private set; } = false;
    public SeedData plantedSeed { get; private set; } = null;
    public bool isGrowing { get; private set; } = false;
    public bool isReadyToHarvest { get; private set; } = false;

    private Renderer spotRenderer;

    [Header("Visuals (Optional)")]
    public Material availableMaterial; 
    public Material growingMaterial;   
    public Material readyMaterial;     

    [Header("Time Display")] 
    public GameObject plantTimeDisplayPrefab; 
    private PlantTimeDisplay currentPlantTimeDisplay;

    private void Awake()
    {
        spotRenderer = GetComponent<Renderer>();
        UpdateVisualState();

        if (plantTimeDisplayPrefab != null)
        {
            GameObject displayObj = Instantiate(plantTimeDisplayPrefab, transform.position + Vector3.up * 1f, Quaternion.identity, transform);
            currentPlantTimeDisplay = displayObj.GetComponent<PlantTimeDisplay>();
            if (currentPlantTimeDisplay == null)
            {
                Debug.LogError("O plantTimeDisplayPrefab não tem o script PlantTimeDisplay!", plantTimeDisplayPrefab);
            }
            currentPlantTimeDisplay.Hide();
        }
    }
    public bool TryPlantSeed(SeedData seedToPlant)
    {
        if (isOccupied) { return false; }
        if (seedToPlant == null) { return false; }

        plantedSeed = seedToPlant;
        isOccupied = true;
        isGrowing = true;
        isReadyToHarvest = false;

        Debug.Log($"Semente '{seedToPlant.seedName}' plantada em {gameObject.name}!");

        StartCoroutine(GrowPlantCoroutine(seedToPlant.growthTime));

        if (currentPlantTimeDisplay != null)
        {
            currentPlantTimeDisplay.Show(this, seedToPlant.growthTime);
        }

        UpdateVisualState();
        return true;
    }

    private IEnumerator GrowPlantCoroutine(float growthDuration)
    {
        yield return new WaitForSeconds(growthDuration);

        isGrowing = false;
        isReadyToHarvest = true;
        Debug.Log($"Planta de {plantedSeed.seedName} cresceu e está pronta para colher em {gameObject.name}!");

        if (currentPlantTimeDisplay != null)
        {
            currentPlantTimeDisplay.Hide();
        }

        UpdateVisualState();
    }

    public SeedData HarvestPlant()
    {
        if (!isOccupied || !isReadyToHarvest) {return null; }

        SeedData harvestedSeed = plantedSeed;

        if (currentPlantTimeDisplay != null)
        {
            currentPlantTimeDisplay.Hide();
        }

        UpdateVisualState();
        return harvestedSeed;
    }

    private void UpdateVisualState()
    {
        if (spotRenderer != null)
        {
            if (isReadyToHarvest && readyMaterial != null)
            {
                spotRenderer.material = readyMaterial;
            }
            else if (isGrowing && growingMaterial != null)
            {
                spotRenderer.material = growingMaterial;
            }
            else if (!isOccupied && availableMaterial != null)
            {
                spotRenderer.material = availableMaterial;
            }

        }
    }

    private PlayerMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCurrentPlantingSpot(this);

                string actionMessage = "";
                if (isReadyToHarvest)
                {
                    actionMessage = "colher";
                }
                else if (isGrowing)
                {
                    actionMessage = "Planta crescendo...";
                }
                else if (!isOccupied)
                {
                    actionMessage = $"plantar {playerMovement.GetSelectedSeedName()}";
                }

                if (!string.IsNullOrEmpty(actionMessage))
                {
                    InteractionManager.Instance.Show($"Pressione 'E' para {actionMessage}", transform.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerMovement != null)
            {
                playerMovement.ClearCurrentPlantingSpot();
            }
            InteractionManager.Instance.Hide();
        }
    }
}