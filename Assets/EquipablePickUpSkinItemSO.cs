using UnityEngine;

[CreateAssetMenu(fileName = "PickUpSkin_", menuName = "ScriptableObject/Equipable/PickUpSkin")]
public class EquipablePickUpSkinItemSO : EquipableItemSO
{
    public Sprite sprite;

    public GameObject skinPrefab;

    public float pickUpSkinHeight;
}
