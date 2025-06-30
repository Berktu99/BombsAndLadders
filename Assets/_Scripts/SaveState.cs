
[System.Serializable]
public class SaveState
{    public enum SaveStateChangeableVariables
    {
        gold,
        playerName,
        characterSkinsUnlocked,
        characterSkinActive,
        pickUpSkinsUnlocked,
        pickUpSkinActive,
        bombSkinsUnlocked,
        bombSkinActive,
        colorSkinsUnlocked,
        colorSkinActive,
        mapsUnlocked,
        activeMapIndex,
        allMapsClearedArray,
        isMuteToggledOn,
        isVibrateToggledOn,
        rewardPercent,
        nullValue,
    }

    public enum Unlockable
    {
        CharacterSkins,
        PickUpSkins,
        BombSkins,
        ColorSkins,
        Maps,
        Null,
    }
        
    public int goldAmount = 0;
    public string playerName = "Player";

    #region Character Skin
    public int _characterSkinsUnlocked = 1;
    public int _characterSkinActiveIndex = 0;
    #endregion

    #region Pick Up Skin
    public int _pickUpSkinsUnlocked = 1;
    public int _pickUpSkinActiveIndex = 0;
    #endregion

    #region Bomb Skin
    public int _bombSkinsUnlocked = 1;
    public int _bombSkinActiveIndex = 0;
    #endregion

    #region Skin Color
    public int _colorSkinsUnlocked = 1;
    public int _colorSkinActiveIndex = 0;
    #endregion

    #region Maps
    public int mapClearRequirement = 3;
    public int[] allMapsClearedArray = { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,};

    public int _mapsUnlocked = 1;
    public int _mapActiveIndex = 0;
    #endregion

    public int rewardPercent = 0;    

    public bool isMuteToggledOn = false;
    public bool isVibrateToggledOn = true;

    public bool adsAreActive = false;
    public bool paidToRemoveAds = false;
    public bool devAuthAdsRemoval = true;
}
