using UnityEngine;
using System.Collections.Generic;
public class SeedController : MonoBehaviour
{
    public ListSeeds m_seedList;
    public SeedData selectedSeed { get; private set; }

    private void Awake()
    {
        selectedSeed = GetRandomSeed();
        ApplyColorToAllRenderers();
    }

    private void ApplyColorToAllRenderers()
    {
        if (selectedSeed == null)
        {
            Debug.LogWarning($"selectedSeed � nulo. N�o foi poss�vel aplicar a cor em {gameObject.name}.", this);
            return;
        }

        Renderer selfRenderer = GetComponent<Renderer>();
        if (selfRenderer != null)
        {
            selfRenderer.material = new Material(selfRenderer.material);
            selfRenderer.material.color = selectedSeed.color;
        }
        else
        {
            Debug.LogWarning($"Renderer n�o encontrado no GameObject {gameObject.name}. N�o foi poss�vel aplicar a cor da semente no pai.", this);
        }

        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in childRenderers)
        {
            if (childRenderer == selfRenderer)
            {
                continue;
            }
            childRenderer.material = new Material(childRenderer.material);
            childRenderer.material.color = selectedSeed.color;
        }
    }

    private SeedData GetRandomSeed()
    {
        if (m_seedList == null)
        {
            Debug.LogError("ListSeeds n�o atribu�da ao SeedController! Atribua o asset ListSeeds no Inspector.", this);
            return null;
        }

        if (m_seedList.m_seedsList == null || m_seedList.m_seedsList.Count == 0)
        {
            Debug.LogWarning("Lista de sementes vazia ou nula em ListSeeds! Adicione SeedData assets � lista.", this);
            return null;
        }

        int index = Random.Range(0, m_seedList.m_seedsList.Count);
        return m_seedList.m_seedsList[index];
    }
}