using UnityEngine;

public class Unlocker : Singleton<Unlocker>
{
    [SerializeField] private SaveManager _saveManager;

    [SerializeField] private IntVariable _goldAmount;

    [MyBox.Foldout("UNLOCKABLES", true)]
    [SerializeField] private IntVariable _characterSkinsUnlocked;
    [SerializeField] private IntVariable _pickUpSkinsUnlocked;
    [SerializeField] private IntVariable _bombSkinsUnlocked;
    [SerializeField] private IntVariable _colorSkinsUnlocked;
    [MyBox.Separator("Maps Variables")]
    [SerializeField] private IntVariable _mapsUnlocked;
    [SerializeField] private IntVariable _mapActiveIndex;
    [SerializeField] private MapsSO maps;


    private SaveStateChange _s = new SaveStateChange(SaveState.SaveStateChangeableVariables.nullValue, -1000);

    public void unlockStuff(SaveState.Unlockable stuff, int toUnlockIndex, int cost)
    {
        Debug.Log("heyp");
        _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.gold;
        _s.newIntValue = _goldAmount.Value - cost;

        _saveManager.changeSaveState(_s);

        switch (stuff)
        {
            case SaveState.Unlockable.CharacterSkins:
                {
                    _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.characterSkinsUnlocked;
                    _s.newIntValue = _characterSkinsUnlocked.Value | (1 << toUnlockIndex);

                    _saveManager.changeSaveState(_s);

                    break;
                }
            case SaveState.Unlockable.PickUpSkins:
                {
                    _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.pickUpSkinsUnlocked;
                    _s.newIntValue = _pickUpSkinsUnlocked.Value | (1 << toUnlockIndex);

                    _saveManager.changeSaveState(_s);

                    break;
                }
            case SaveState.Unlockable.BombSkins:
                {
                    _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.bombSkinsUnlocked;
                    _s.newIntValue = _bombSkinsUnlocked.Value | (1 << toUnlockIndex);

                    _saveManager.changeSaveState(_s);

                    break;
                }
            case SaveState.Unlockable.ColorSkins:
                {
                    _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.colorSkinsUnlocked;
                    _s.newIntValue = _colorSkinsUnlocked.Value | (1 << toUnlockIndex);

                    _saveManager.changeSaveState(_s);

                    break;
                }
            case SaveState.Unlockable.Maps:
                {
                    _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.mapsUnlocked;
                    _s.newIntValue = _mapsUnlocked.Value | (1 << toUnlockIndex);

                    _saveManager.changeSaveState(_s);


                    if (_mapActiveIndex.Value < maps.availableMaps.Count)
                    {
                        _s.saveStateChangeable = SaveState.SaveStateChangeableVariables.activeMapIndex;
                        _s.newIntValue = _mapActiveIndex.Value + 1;
                        _saveManager.changeSaveState(_s);
                    }

                    break;
                }
        }
    }
}
