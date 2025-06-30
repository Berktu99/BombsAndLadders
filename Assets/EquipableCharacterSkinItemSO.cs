using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkin_", menuName = "ScriptableObject/Equipable/CharacterSkin")]
public class EquipableCharacterSkinItemSO : EquipableItemSO
{
    public Sprite sprite;

    public HumanoidSkinType skinType;
    public Vector3 pickupPackPos;

    public GameObject skinPrefab;
}
