using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleMap
{
    public string mapName;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "Maps", menuName = "ScriptableObject/Maps")]
public class MapsSO : ScriptableObject
{
    public List<SingleMap> availableMaps = new List<SingleMap>();

    public List<SingleMap> workInProgressMaps = new List<SingleMap>();

    public List<SingleMap> comingSoonMaps = new List<SingleMap>();
}
