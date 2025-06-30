using UnityEngine;

public class PlusReward : MonoBehaviour
{
    [SerializeField] private SaveStateChangeEvent saveStateChange;

    [SerializeField] private IntVariable currentRewardPercent;

    [SerializeField] private IntVariable plusRewardPerAd;

    public void OnAdWatched()
    {
        saveStateChange.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.rewardPercent, (currentRewardPercent.Value + plusRewardPerAd.Value) % 100));
    }
}
