using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListSeeds", menuName = "Scriptable Objects/Seed/ListSeeds")]
public class ListSeeds : ScriptableObject
{
    public new List<SeedData> m_seedsList = new List<SeedData>();
}
