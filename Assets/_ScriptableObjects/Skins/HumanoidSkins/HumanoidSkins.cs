using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanoidSkinType
{
    Accessory,
    WholeBody,
}


[System.Serializable]
public class SingleHumanoidSkin : SingleSkin, IAvailableItem
{
    public Sprite sprite;
    
    public HumanoidSkinType skinType;
    public Vector3 pickupPackPos;

    public GameObject skinPrefab;

    public HumanoidEquipEvent equipSkinEvent;
    public void equip()
    {
        equipSkinEvent.Raise(new HumanoidEquip(skinType, skinPrefab));
    }
}

[CreateAssetMenu(fileName = "HumanoidSkins", menuName = "ScriptableObject/Skins/HumanoidSkins")]
public class HumanoidSkins : ScriptableObject
{
    public List<SingleHumanoidSkin> availableSkins = new List<SingleHumanoidSkin>();

    public List<SingleHumanoidSkin> workInProgressSkins = new List<SingleHumanoidSkin>();

    public List<SingleHumanoidSkin> comingSoonSkins = new List<SingleHumanoidSkin>();    
}
