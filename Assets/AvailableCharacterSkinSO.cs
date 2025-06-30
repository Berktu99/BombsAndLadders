using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "CharacterSkin_Available_", menuName = "ScriptableObject/Equipable/CharacterSkin/Available")]
public class AvailableCharacterSkinSO : EquipableCharacterSkinItemSO, IAvailableItem
{
    [Separator("Humanoid Equip Events", true)]
    public HumanoidEquipEvent equipSkinEvent;
    public HumanoidEquipEvent equipColorEvent;

    [Separator("SaveStateChange Event", true)]
    public SaveStateChangeEvent saveStateChangeEvent;

    public void equip()
    {
        equipSkinEvent.Raise(new HumanoidEquip(skinType, skinPrefab));

        //equipColorEvent.Raise(new HumanoidEquip(allSkins.colorSkins.availableSkins[saveStateObject.colorSkinActiveIndex.Value].skinColor));
    }
}
