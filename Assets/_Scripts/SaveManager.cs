using System;
using UnityEngine;
using MyBox;

public class SaveManager : MonoBehaviour
{ 
    [HideInInspector] public SaveState saveState;
    private string saveFilePath;

    public Action<SaveState> onLoad;
    public Action<SaveState> onSave;

    [SerializeField] private bool shouldResetSave = false;

    [Foldout("Scriptable Object Stuff", true)]
    public SaveStateObject saveStateObject;
    [SerializeField] private AllSkins allSkins;
    [SerializeField] private MapsSO maps;
    [SerializeField] private VoidEvent updateGoldAmountText;
    [SerializeField] private BoolVariable shouldForceActivateAds;

    private SaveStateChange s = new SaveStateChange(SaveState.SaveStateChangeableVariables.nullValue, -1);

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/" + "data.txt";

        if (PlayerPrefs.HasKey("adsWereForceActivated"))
        {
            PlayerPrefs.SetInt("adsWereForceActivated", 0);
        }

        //if (shouldResetSave)
        //    resetSave();

        load();

        //purchaseGold(10000);

        matchScriptableObjectsWithSaveData();
    }

    private void matchScriptableObjectsWithSaveData()
    {
        try
        {
            saveState = SerializeHelper.deserializeFromJsonSafe<SaveState>(saveFilePath);

            match();
            save();
        }
        catch
        {
            if (saveState == null)
            {
                saveState = new SaveState();
            }

            match();
            save();

            Debug.Log("No save file was found, creating a new save file.");

        }

        void match()
        {
            saveStateObject._goldAmount.SetValue(saveState.goldAmount);
            saveStateObject._currentRewardPercent.SetValue(saveState.rewardPercent);

            saveStateObject._playerName.SetValue(saveState.playerName);

            saveStateObject._characterSkinActiveIndex.SetValue(saveState._characterSkinActiveIndex);
            saveStateObject._bombSkinActiveIndex.SetValue(saveState._bombSkinActiveIndex);
            saveStateObject._pickUpSkinActiveIndex.SetValue(saveState._pickUpSkinActiveIndex);
            saveStateObject._colorSkinActiveIndex.SetValue(saveState._colorSkinActiveIndex);

            saveStateObject._characterSkinsUnlocked.SetValue(saveState._characterSkinsUnlocked);
            saveStateObject._bombSkinsUnlocked.SetValue(saveState._bombSkinsUnlocked);
            saveStateObject._pickUpSkinsUnlocked.SetValue(saveState._pickUpSkinsUnlocked);
            saveStateObject._colorSkinsUnlocked.SetValue(saveState._colorSkinsUnlocked);

            saveStateObject._mapClearRequirement.SetValue(saveState.mapClearRequirement);
            saveStateObject._mapActiveIndex.SetValue(saveState._mapActiveIndex);
            saveStateObject._currentMapClearedTimes.SetValue(saveState.allMapsClearedArray[saveState._mapActiveIndex]);
            saveStateObject._mapsUnlocked.SetValue(saveState._mapsUnlocked);            

            saveStateObject.isMuteToggledOn.SetValue(saveState.isMuteToggledOn);
            saveStateObject.isVibrateToggledOn.SetValue(saveState.isVibrateToggledOn);

            saveStateObject.adsAreActive.SetValue(saveState.adsAreActive);
            saveStateObject.devAuthAdsRemoval.SetValue(saveState.devAuthAdsRemoval);
            saveStateObject.paidToRemoveAds.SetValue(saveState.paidToRemoveAds);
        }
    }

    private void save()
    {
        if (saveState == null)
        {
            saveState = new SaveState();
        }

        if (shouldForceActivateAds.Value == true && PlayerPrefs.GetInt("adsWereForceActivated") == 0)
        {
            forceActivateAds();
        }

        SerializeHelper.serializeToJsonSafe(saveState, saveFilePath);

        onSave?.Invoke(saveState);
    }

    public void load()
    {
        try
        {
            saveState = SerializeHelper.deserializeFromJsonSafe<SaveState>(saveFilePath);
            onLoad?.Invoke(saveState);
        }
        catch
        {
            Debug.Log("No save file was found, creating a new save file.");
            save();
        }
    }

    private void forceActivateAds()
    {
        saveState.adsAreActive = true;
        saveState.devAuthAdsRemoval = false;
        saveState.paidToRemoveAds = false;

        PlayerPrefs.SetInt("adsWereForceActivated", 1);

        matchScriptableObjectsWithSaveData();
    }

    public void resetSave()
    {
        System.IO.File.Delete(saveFilePath);

        for (int i = 0; i < allSkins.characterSkins.availableSkins.Count; i++)
        {
            allSkins.characterSkins.availableSkins[i].videoWatched = 0;
        }

        for (int i = 0; i < allSkins.pickUpSkins.availableSkins.Count; i++)
        {
            allSkins.pickUpSkins.availableSkins[i].videoWatched = 0;
        }

        for (int i = 0; i < allSkins.bombSkins.availableSkins.Count; i++)
        {
            allSkins.bombSkins.availableSkins[i].videoWatched = 0;
        }

        for (int i = 0; i < allSkins.colorSkins.availableSkins.Count; i++)
        {
            allSkins.colorSkins.availableSkins[i].videoWatched = 0;
        }
    }

    public void changeSaveState(SaveStateChange saveStateChange)
    {
        int newIntValue = saveStateChange.newIntValue;

        switch (saveStateChange.saveStateChangeable)
        {
            case SaveState.SaveStateChangeableVariables.playerName:
                {
                    saveState.playerName = saveStateChange.newStringValue;
                    saveStateObject._playerName.SetValue(saveStateChange.newStringValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.gold:
                {
                    Debug.Log("new gold amount : " + newIntValue);
                    saveState.goldAmount = newIntValue;
                    saveStateObject._goldAmount.SetValue(newIntValue);

                    updateGoldAmountText.Raise();
                    break;
                }
            case SaveState.SaveStateChangeableVariables.characterSkinsUnlocked:
                {
                    saveState._characterSkinsUnlocked = newIntValue;
                    saveStateObject._characterSkinsUnlocked.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.characterSkinActive:
                {
                    saveState._characterSkinActiveIndex = newIntValue;
                    saveStateObject._characterSkinActiveIndex.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.pickUpSkinsUnlocked:
                {
                    saveState._pickUpSkinsUnlocked = newIntValue;
                    saveStateObject._pickUpSkinsUnlocked.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.pickUpSkinActive:
                {
                    saveState._pickUpSkinActiveIndex = newIntValue;
                    saveStateObject._pickUpSkinActiveIndex.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.bombSkinsUnlocked:
                {
                    saveState._bombSkinsUnlocked = newIntValue;
                    saveStateObject._bombSkinsUnlocked.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.bombSkinActive:
                {
                    saveState._bombSkinActiveIndex = newIntValue;
                    saveStateObject._bombSkinActiveIndex.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.colorSkinsUnlocked:
                {
                    saveState._colorSkinsUnlocked = newIntValue;
                    saveStateObject._colorSkinsUnlocked.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.colorSkinActive:
                {
                    saveState._colorSkinActiveIndex = newIntValue;
                    saveStateObject._colorSkinActiveIndex.SetValue(newIntValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.rewardPercent:
                {
                    saveState.rewardPercent = newIntValue;
                    saveStateObject._currentRewardPercent.SetValue(newIntValue);
                    break;
                }               
            case SaveState.SaveStateChangeableVariables.allMapsClearedArray:
                {
                    saveState.allMapsClearedArray[saveState._mapActiveIndex] += newIntValue;
                    saveStateObject._currentMapClearedTimes.SetValue(saveState.allMapsClearedArray[saveState._mapActiveIndex]);

                    if (saveState.allMapsClearedArray[saveState._mapActiveIndex] >= saveState.mapClearRequirement)
                    {
                        saveState.allMapsClearedArray[saveState._mapActiveIndex] = saveState.mapClearRequirement;                        
                        saveStateObject._currentMapClearedTimes.SetValue(saveState.allMapsClearedArray[saveState._mapActiveIndex]);

                        int lastUnlockedMapIndex = 0;
                        while (true)
                        {
                            if (( saveState._mapsUnlocked & (1 << (lastUnlockedMapIndex) )) == 0)
                            {
                                lastUnlockedMapIndex--;
                                break;
                            }
                            else
                            {
                                lastUnlockedMapIndex++;
                            }                            
                        }

                        if (saveState._mapActiveIndex == lastUnlockedMapIndex)
                        {
                            if (maps.availableMaps.Count - 1 > lastUnlockedMapIndex)
                            {
                                // Unlock Next Map
                                saveState._mapActiveIndex++;
                                saveStateObject._mapActiveIndex.SetValue(saveState._mapActiveIndex);

                                saveState._mapsUnlocked |= (1 << (saveState._mapActiveIndex));
                                saveStateObject._mapsUnlocked.SetValue(saveState._mapsUnlocked);
                            }
                        }
                    }

                    break;
                }            
            case SaveState.SaveStateChangeableVariables.isMuteToggledOn:
                {
                    saveState.isMuteToggledOn = saveStateChange.newBoolValue;
                    saveStateObject.isMuteToggledOn.SetValue(saveStateChange.newBoolValue);
                    break;
                }
            case SaveState.SaveStateChangeableVariables.isVibrateToggledOn:
                {
                    saveState.isVibrateToggledOn = saveStateChange.newBoolValue;
                    saveStateObject.isVibrateToggledOn.SetValue(saveStateChange.newBoolValue);
                    break;
                }
            default:
                {
                    Debug.Log("You did not account for this. : " + saveStateChange.saveStateChangeable);
                }
                break;
        }

        save();
    }    

    public void purchaseGold(int amountToAdd)
    {
        SaveStateChange s = new SaveStateChange(SaveState.SaveStateChangeableVariables.gold, saveState.goldAmount + amountToAdd);
        changeSaveState(s);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (pause)
    //        matchDataWithScriptableObjects();
    //}

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (!focus)
    //    {
    //        matchDataWithScriptableObjects();
    //    }
    //}

    //protected override void OnApplicationQuit()
    //{
    //    matchDataWithScriptableObjects();

    //    base.OnApplicationQuit();
    //}
}
