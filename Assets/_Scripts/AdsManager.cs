using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : Singleton<AdsManager>
{
    [SerializeField] private string gameID;
    [SerializeField] private string rewardedVideoPlacementId;
    [SerializeField] private bool testMode;


    protected override void Awake()
    {
        base.Awake();

        Advertisement.Initialize(gameID, testMode);
    }

    public void ShowRewardedAd()
    {
        ShowOptions so = new ShowOptions();
        Advertisement.Show(rewardedVideoPlacementId, so);
    }

}