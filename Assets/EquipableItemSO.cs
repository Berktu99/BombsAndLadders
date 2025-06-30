using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "NewEquipableItem", menuName = "ScriptableObject/Equipable/Item")]
public class EquipableItemSO : ScriptableObject
{
    public string itemName;

    public bool isCostVideo = false;

    [ConditionalField(nameof(isCostVideo), false)] public int videoCost;
    [ConditionalField(nameof(isCostVideo), false)] public int videoWatched;

    [ConditionalField(nameof(isCostVideo), true)] public int cost;
}
