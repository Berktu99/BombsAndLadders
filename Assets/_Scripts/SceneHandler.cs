using UnityEngine;
using MyBox;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private SaveStateChangeEvent saveStateChangeEvent;

    [Foldout("Map Scene and Level Stuff", true)]
    [SerializeField] private IntVariable currentMapCleared;
    [SerializeField] private IntVariable notUpdated_currentMapCleared;
    [SerializeField] private IntVariable updated_currentMapCleared;
    [SerializeField] private IntVariable _activeMapIndex;


    [SerializeField] private IntVariable _mapsUnlocked;
    [SerializeField] private IntVariable _mapsUnlocked_NotUpdated;

    [Foldout("Save Before Loading", true)]
    [SerializeField] private IntVariable earnedGold;
    [SerializeField] private IntVariable currentGoldAmount;
    [SerializeField] private IntVariable currentRewardPercent;
    

    private void Start()
    {
        _mapsUnlocked_NotUpdated.SetValue(_mapsUnlocked.Value);

        notUpdated_currentMapCleared.SetValue(currentMapCleared.Value);
        updated_currentMapCleared.SetValue(currentMapCleared.Value);
    }

    public void LoadScene()
    {
        StartCoroutine(wait());
        System.Collections.IEnumerator wait()
        {
            yield return Yielders.CachedWaitForSeconds(0.2f);

            SaveProgress();
            Loader.loadMap(_activeMapIndex.Value);
        }        
    }

    public void SaveProgress()
    {
        // this is a fail safe
        // every thing is already saved in save manager
        // for purposes of being fair to player
        
        //// save the gold earned
        //saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.gold,  earnedGold.Value + currentGoldAmount.Value));


        //saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.rewardPercent, currentRewardPercent.Value));
        //// save the what level i guess..
        //Debug.Log("save the progress on level, which is not implemented yet, like at all dude it is so much work to do this shit fuuuuuck.");
    }

    public void UpdateMap()
    {
        //_originalActiveMapIndex.SetValue(_activeMapIndex.Value);
        saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.allMapsClearedArray, 1));
        updated_currentMapCleared.SetValue(currentMapCleared.Value);
    }
    
}
