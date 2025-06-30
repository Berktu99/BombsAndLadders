using UnityEngine;
using UnityEngine.Advertisements;
using MyBox;

public class RewardedAdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Foldout("AD Variables", true)]
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    private string _adUnitId = null; // This will remain null for unsupported platforms

    [Foldout("Child Fields", true)]
    [SerializeField] private MultiplyGold multiplyGold;
    [SerializeField] private PlusReward plusReward;
    [SerializeField] private SkipLevel _skipLevel;

    [Foldout("Scriptable Events", true)]
    [SerializeField] private VoidEvent OnRewardVideoWatched_Skin;    
    [SerializeField] private VoidEvent OnRewardVideoWatched_SkipLevel;
    [SerializeField] private VoidEvent OnRewardVideoWatched_ContinueLevel;
    [SerializeField] private VoidEvent OnRewardVideoWatched_MultGold;
    [SerializeField] private VoidEvent OnRewardVideoWatched_PlusReward;
    [SerializeField] private VoidEvent OnRewardVideoWASNTwatched;

    private int multGoldAmount = 0;
    private enum AdForWhat
    {
        none,
        multEarnedGold,
        plusReward,
        skipLevel,
        continueLevel,
        skin,
    }

    private AdForWhat forWhat = AdForWhat.none;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        if (_iOSAdUnitId == "asd")
        {
            Debug.Log("fuck this warning making me do this stupid line shit");
        }
    }

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd_Skin()
    {
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        forWhat = AdForWhat.skin;
    }

    public void ShowAd_MultiplyGold(int multGoldAmount)
    {
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        forWhat = AdForWhat.multEarnedGold;
        this.multGoldAmount = multGoldAmount;
    }

    public void ShowAd_PlusReward()
    {
        Advertisement.Show(_adUnitId, this);
        forWhat = AdForWhat.plusReward;
    }

    public void ShowAd_ContinueLevel()
    {
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        forWhat = AdForWhat.continueLevel;
    }

    public void ShowAd_SkipLevel()
    {
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        forWhat = AdForWhat.skipLevel;
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed. Grant Reward pls");
            // Grant a reward.

            switch (forWhat)
            {
                case AdForWhat.multEarnedGold:
                    {
                        Debug.Log("multiply gold with : " + multGoldAmount);

                        multiplyGold.OnMultiplyGold(multGoldAmount);
                        multGoldAmount = 0;

                        OnRewardVideoWatched_MultGold.Raise();

                        break;
                    }                    
                case AdForWhat.plusReward:
                    {
                        plusReward.OnAdWatched();
                        OnRewardVideoWatched_PlusReward.Raise();

                        break;
                    }                    
                case AdForWhat.skipLevel:
                    {
                        _skipLevel.SkipThisLevel();
                        OnRewardVideoWatched_SkipLevel.Raise();
                        break;
                    }                    
                case AdForWhat.continueLevel:
                    {
                        OnRewardVideoWatched_ContinueLevel.Raise();
                        break;
                    }                    
                case AdForWhat.skin:
                    {
                        OnRewardVideoWatched_Skin.Raise();
                        break;
                    }                    
                default:
                    Debug.LogWarning("this should not have happened");
                    break;
            }
            forWhat = AdForWhat.none;

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
        OnRewardVideoWASNTwatched.Raise();
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
        OnRewardVideoWASNTwatched.Raise();
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }    
}