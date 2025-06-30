using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SinglePickUpSkin : SingleSkin
{
    public Sprite sprite;

    public GameObject skinPrefab;

    public float pickUpSkinHeight;
}


[CreateAssetMenu(fileName = "PickUpSkins", menuName = "ScriptableObject/Skins/PickUpSkins")]
public class PickUpSkins : ScriptableObject
{
    public List<SinglePickUpSkin> availableSkins = new List<SinglePickUpSkin>();

    public List<SinglePickUpSkin> workInProgressSkins = new List<SinglePickUpSkin>();

    public List<SinglePickUpSkin> comingSoonSkins = new List<SinglePickUpSkin>();
}
