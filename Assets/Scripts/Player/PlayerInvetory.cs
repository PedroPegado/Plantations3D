using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<SeedData> seeds = new List<SeedData>();

    public void AddSeed(SeedData seed)
    {
        seeds.Add(seed);
        Debug.Log($"Pegou a semente: {seed.seedName}");
    }
}
