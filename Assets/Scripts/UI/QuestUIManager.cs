using UnityEngine;
using TMPro; 
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI interactionPromptText;
    public Image backgroundQuest;

    [Header("Quest States")]
    public bool hasPickedUpFirstSeed = false;
    public bool hasPlantedFirstSeed = false;
    public bool hasHarvestedFirstPlant = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UpdateQuestUI();
        backgroundQuest.transform.DOMoveX(-50, .5f).SetEase(Ease.OutQuad);
    }

    public void ShowInteractionPrompt(string message)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = message;
            interactionPromptText.gameObject.SetActive(true);
        }
    }


    public void HideInteractionPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
            backgroundQuest.gameObject.SetActive(false);
        }
    }


    public void OnSeedPickedUp()
    {
        if (!hasPickedUpFirstSeed)
        {
            hasPickedUpFirstSeed = true;
            Debug.Log("Primeira semente pega! Atualizando UI");
            UpdateQuestUI();
        }
    }

    public void OnSeedPlanted()
    {
        if (hasPickedUpFirstSeed && !hasPlantedFirstSeed) 
        {
            hasPlantedFirstSeed = true;
            Debug.Log("Primeira semente plantada! Atualizando UI");
            UpdateQuestUI();
        }
    }

    public void OnPlantHarvested()
    {
        if (hasPlantedFirstSeed && !hasHarvestedFirstPlant) 
        {
            hasHarvestedFirstPlant = true;
            Debug.Log("Primeira planta colhida! Quest completa!");
            UpdateQuestUI();
        }
    }

    public void UpdateQuestUI()
    {
        if (!hasPickedUpFirstSeed)
        {
            ShowInteractionPrompt("Encontre e pegue uma semente no chão, cada semente tem um tempo diferente para crescer!");
        }
        else if (hasPickedUpFirstSeed && !hasPlantedFirstSeed)
        {
            ShowInteractionPrompt("Encontre um local de plantio e plante a semente (Pressione E)!");
        }
        else if (hasPlantedFirstSeed && !hasHarvestedFirstPlant)
        {
            ShowInteractionPrompt("Aguarde a planta crescer para colher (Pressione E)!");
        }
        else if (hasHarvestedFirstPlant)
        {
            ShowInteractionPrompt("Parabéns! Quest de plantio concluída!");
            StartCoroutine(HidePromptAfterDelay(5f));
        }
    }

    private IEnumerator HidePromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        backgroundQuest.transform.DOMoveX(-700, .5f).SetEase(Ease.OutQuad);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        HideInteractionPrompt();
    }
}