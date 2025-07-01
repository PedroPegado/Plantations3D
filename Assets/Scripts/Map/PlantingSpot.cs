using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.VFX;

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

    private GameObject currentPlantModelInstance;
    private Renderer currentPlantModelRenderer;

    [Header("Growth Stages")]
    [Range(0, 1)] public float sproutStagePercentage = 0.3f;
    [Range(0, 1)] public float adultStagePercentage = 0.6f;

    [Header("Sprout Animation")]
    public float sproutAppearDuration = 0.5f;

    [Header("Adult Plant Animation")]
    public float initialAdultYPosition = -6f;
    public float finalAdultYPosition = 0f;

    [Header("VFX")] 
    public VisualEffect growthCompleteVFXPrefab;

    private PlayerMovement playerMovement;

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
        if (isOccupied) { Debug.LogWarning("Este local já está ocupado."); return false; }
        if (seedToPlant == null) { Debug.LogWarning("Tentou plantar uma semente nula."); return false; }

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

        if (QuestUIManager.Instance != null)
        {
            QuestUIManager.Instance.OnSeedPlanted();
        }

        return true;
    }

    private IEnumerator GrowPlantCoroutine(float growthDuration)
    {
        float timer = 0f;

        Debug.Log($"Iniciando crescimento para {plantedSeed.seedName}. Duração: {growthDuration}s");

        bool sproutInstantiated = false;
        bool adultInstantiated = false;

        while (timer < growthDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / growthDuration;

            if (progress >= adultStagePercentage)
            {
                if (!adultInstantiated)
                {
                    Debug.Log("Transição para estágio ADULTO. Instanciando e iniciando animações.");

                    if (currentPlantModelInstance != null && currentPlantModelInstance == plantedSeed.sproutPrefab)
                    {
                        currentPlantModelInstance.transform.DOScale(Vector3.zero, sproutAppearDuration)
                            .SetEase(Ease.InQuad)
                            .OnComplete(() => DestroyCurrentPlantModel(false))
                            .SetLink(currentPlantModelInstance.gameObject);

                        yield return new WaitForSeconds(sproutAppearDuration);
                    }
                    else
                    {
                        DestroyCurrentPlantModel(true);
                    }

                    currentPlantModelInstance = Instantiate(plantedSeed.adultPlantPrefab, transform.position, Quaternion.identity, transform);
                    currentPlantModelRenderer = currentPlantModelInstance.GetComponentInChildren<Renderer>();

                    Vector3 startLocalPos = currentPlantModelInstance.transform.localPosition;
                    startLocalPos.y = initialAdultYPosition;
                    currentPlantModelInstance.transform.localPosition = startLocalPos;

                    currentPlantModelInstance.transform.localScale = Vector3.zero;

                    float remainingGrowthTime = growthDuration * (1f - adultStagePercentage);

                    currentPlantModelInstance.transform.DOLocalMoveY(finalAdultYPosition, remainingGrowthTime)
                        .SetEase(Ease.OutQuad)
                        .SetLink(currentPlantModelInstance.gameObject);

                    currentPlantModelInstance.transform.DOScale(Vector3.one * 1.5f, remainingGrowthTime) 
                        .SetEase(Ease.OutQuad)
                        .SetLink(currentPlantModelInstance.gameObject)
                        .OnComplete(() => Debug.Log("Animação de crescimento adulta COMPLETA."));

                    adultInstantiated = true;
                }
            }
            else if (progress >= sproutStagePercentage)
            {
                if (!sproutInstantiated)
                {
                    Debug.Log("Transição para estágio BROTINHO. Instanciando e iniciando animações.");
                    DestroyCurrentPlantModel(true); 
                    currentPlantModelInstance = Instantiate(plantedSeed.sproutPrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity, transform);
                    currentPlantModelRenderer = currentPlantModelInstance.GetComponentInChildren<Renderer>();

                    currentPlantModelInstance.transform.localScale = Vector3.zero; 
                    currentPlantModelInstance.transform.DOScale(Vector3.one * 1.5f, sproutAppearDuration)
                        .SetEase(Ease.OutQuad)
                        .SetLink(currentPlantModelInstance.gameObject);

                    sproutInstantiated = true;
                }

                if (currentPlantModelRenderer != null && currentPlantModelRenderer.material != null)
                {
                    if (!currentPlantModelRenderer.material.name.Contains(" (Instance)"))
                    {
                        currentPlantModelRenderer.material = new Material(currentPlantModelRenderer.material);
                    }

                    float deadColorProgress = Mathf.InverseLerp(sproutStagePercentage, adultStagePercentage, progress);
                    Color deadColor = plantedSeed.color;
                    Color.RGBToHSV(plantedSeed.color, out float h, out float s, out float v);
                    deadColor = Color.HSVToRGB(h, s * 0.5f, v * 0.5f);
                    deadColor.a = 1f;

                    currentPlantModelRenderer.material.color = Color.Lerp(deadColor, plantedSeed.color, deadColorProgress);
                }
            }
            else
            {
                DestroyCurrentPlantModel(true);
                sproutInstantiated = false;
                adultInstantiated = false;
            }

            yield return null;
        }

        Debug.Log($"Fim do crescimento para {plantedSeed.seedName}. Garantindo estado final.");

        if (currentPlantModelInstance == null || currentPlantModelInstance != plantedSeed.adultPlantPrefab)
        {
            Debug.Log("Instanciando planta adulta final no 100% (fallback).");
            DestroyCurrentPlantModel(true);
            currentPlantModelInstance = Instantiate(plantedSeed.adultPlantPrefab, transform.position, Quaternion.identity, transform);
            currentPlantModelRenderer = currentPlantModelInstance.GetComponentInChildren<Renderer>();
        }

        currentPlantModelInstance.transform.localScale = Vector3.one * 1.5f; // Escala final ajustada
        Vector3 finalPos = currentPlantModelInstance.transform.localPosition;
        finalPos.y = finalAdultYPosition;
        currentPlantModelInstance.transform.localPosition = finalPos;

        if (currentPlantModelInstance != null)
        {
            currentPlantModelInstance.transform.DOKill(true);
        }

        isGrowing = false;
        isReadyToHarvest = true;
        Debug.Log($"Planta de {plantedSeed.seedName} cresceu e está pronta para colher em {gameObject.name}!");
        growthCompleteVFXPrefab.Play();

        if (currentPlantTimeDisplay != null)
        {
            currentPlantTimeDisplay.Hide();
        }
        UpdateVisualState();
    }

    public SeedData HarvestPlant()
    {
        if (!isOccupied || !isReadyToHarvest)
        {
            Debug.LogWarning("Não há nada para colher ou a planta ainda não cresceu.");
            return null;
        }

        growthCompleteVFXPrefab.Stop();
        SeedData harvestedSeed = plantedSeed;

        isOccupied = false;
        plantedSeed = null;
        isGrowing = false;
        isReadyToHarvest = false;

        Debug.Log($"Planta de {harvestedSeed.seedName} colhida em {gameObject.name}!");

        DestroyCurrentPlantModel(true);
        if (currentPlantTimeDisplay != null) { currentPlantTimeDisplay.Hide(); }
        UpdateVisualState();

        if (QuestUIManager.Instance != null)
        {
            QuestUIManager.Instance.OnPlantHarvested();
        }

        return harvestedSeed;
    }


    private void DestroyCurrentPlantModel(bool killTweensImmediately)
    {
        if (currentPlantModelInstance != null)
        {
            if (killTweensImmediately)
            {
                currentPlantModelInstance.transform.DOKill(true);
            }
            Destroy(currentPlantModelInstance);
            currentPlantModelInstance = null;
            currentPlantModelRenderer = null;
        }
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