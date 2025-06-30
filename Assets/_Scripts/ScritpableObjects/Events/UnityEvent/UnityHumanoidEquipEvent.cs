using UnityEngine.Events;

[System.Serializable]
public class UnityHumanoidEquipEvent : UnityEvent<HumanoidEquip>
{
    
}

public class HumanoidEquip
{
    public HumanoidSkinType humanoidSkinType;
    public UnityEngine.GameObject skinPrefab;
    public UnityEngine.Color32 colorSkin;

    public HumanoidEquip(HumanoidSkinType humanoidSkinType, UnityEngine.GameObject skinPrefab)
    {
        this.humanoidSkinType = humanoidSkinType;
        this.skinPrefab = skinPrefab;
    }

    public HumanoidEquip(UnityEngine.Color32 colorSkin)
    {
        this.colorSkin = colorSkin;
    }
}