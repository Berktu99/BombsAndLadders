using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SingleColorSkin : SingleSkin
{
    public Color32 skinColor;
}

[CreateAssetMenu(fileName = "ColorSkins", menuName = "ScriptableObject/Skins/ColorSkins")]
public class ColorSkins : ScriptableObject
{
    public List<SingleColorSkin> availableSkins = new List<SingleColorSkin>();

    public List<SingleColorSkin> workInProgressSkins = new List<SingleColorSkin>();

    public List<SingleColorSkin> comingSoonSkins = new List<SingleColorSkin>();
}
