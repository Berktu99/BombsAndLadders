using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleBombSkin : SingleSkin
{
    public Sprite sprite;
    public GameObject skinPrefab;

    public Vector3 pulseLocalScale;
}


[CreateAssetMenu(fileName = "BombSkins", menuName = "ScriptableObject/Skins/BombSkins")]
public class BombSkins : ScriptableObject
{
    public List<SingleBombSkin> availableSkins = new List<SingleBombSkin>();

    public List<SingleBombSkin> workInProgressSkins = new List<SingleBombSkin>();

    public List<SingleBombSkin> comingSoonSkins = new List<SingleBombSkin>();
}
