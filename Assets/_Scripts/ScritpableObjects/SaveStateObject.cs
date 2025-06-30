using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "SaveState", menuName = "ScriptableObject/Saving")]

public class SaveStateObject : ScriptableObject
{
    [Foldout("Int Variables", true)]
    public IntVariable _goldAmount;
    public IntVariable _currentRewardPercent;

    [Foldout("String Variables", true)]
    public StringVariable _playerName;

    #region Character Skin
    [Foldout("Character Skin", true)]    
    public IntVariable _characterSkinsUnlocked;
    public IntVariable _characterSkinActiveIndex;
    #endregion

    #region Pick Up Skin
    [Foldout("PickUpSkin", true)]    
    public IntVariable _pickUpSkinsUnlocked;
    public IntVariable _pickUpSkinActiveIndex;
    #endregion

    #region Bomb Skin
    [Foldout("Bomb Skin", true)]    
    public IntVariable _bombSkinsUnlocked;
    public IntVariable _bombSkinActiveIndex;
    #endregion

    #region Skin Color
    [Foldout("Skin Color", true)]    
    public IntVariable _colorSkinsUnlocked;
    public IntVariable _colorSkinActiveIndex;
    #endregion

    #region Maps
    [Foldout("Maps", true)]
    public IntVariable _mapClearRequirement;    
    public IntVariable _currentMapClearedTimes;

    public IntVariable _mapsUnlocked;
    public IntVariable _mapActiveIndex;
    #endregion

    #region Bool Variables
    [MyBox.Foldout("Bool Variables", true)]
    public BoolVariable isMuteToggledOn;
    public BoolVariable isVibrateToggledOn;
    public BoolVariable adsAreActive;
    public BoolVariable paidToRemoveAds;
    public BoolVariable devAuthAdsRemoval; 
    #endregion

    public bool isUnlocked(SaveState.Unlockable stuff, int index)
    {
        switch (stuff)
        {
            case SaveState.Unlockable.CharacterSkins:
                {
                    return (_characterSkinsUnlocked.Value & (1 << index)) != 0;
                }
            case SaveState.Unlockable.PickUpSkins:
                {
                    return (_pickUpSkinsUnlocked.Value & (1 << index)) != 0;
                }
            case SaveState.Unlockable.BombSkins:
                {
                    return (_bombSkinsUnlocked.Value & (1 << index)) != 0;
                }
            case SaveState.Unlockable.ColorSkins:
                {
                    return (_colorSkinsUnlocked.Value & (1 << index)) != 0;
                }
            case SaveState.Unlockable.Maps:
                {                    
                    return (_mapsUnlocked.Value & (1 << index)) != 0;
                }
            default:
                {
                    Debug.Log("You tried to check an unknown purchasable stuff.");
                    return false;
                }
        }
    }
   
}
