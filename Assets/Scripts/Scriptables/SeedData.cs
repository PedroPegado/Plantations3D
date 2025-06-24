using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Plantation/Seed")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public float growthTime;
    public Sprite icon;
}
