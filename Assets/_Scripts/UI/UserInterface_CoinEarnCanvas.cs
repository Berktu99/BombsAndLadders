using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class UserInterface_CoinEarnCanvas : MonoBehaviour
{
    [SerializeField] private IntVariable goldEarned;
    [SerializeField] private IntVariable goldEarnedUpdated;

    [SerializeField] private GameObject goldCoin;
    [SerializeField] private Camera coinOverlayCamera;
    
    [SerializeField] private VoidEvent finishedCoinEarnCanvas;

    [SerializeField] private RectTransform goldGui;
    [SerializeField] private RectTransform begin;

    [SerializeField] private int coinsToCreateAmount = 20;

    [SerializeField] private float spawnRadius = 10f;

    [SerializeField] private float coinSpawnTime;
    [SerializeField] private float coinSpawnAccel;

    [SerializeField] private float endTime = 2f;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }
    private void OnEnable()
    {        
        if (!Camera.main.GetUniversalAdditionalCameraData().cameraStack.Contains(coinOverlayCamera)) 
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(coinOverlayCamera);
        }

        StartCoroutine(CoinFlyRoutine());
    }

    private IEnumerator CoinFlyRoutine()
    {
        yield return Yielders.CachedWaitForSeconds(0.1f);//new WaitForSeconds(Time.unscaledDeltaTime);

        int value = (goldEarned.Value - goldEarned.Value % coinsToCreateAmount) / coinsToCreateAmount + goldEarned.Value % coinsToCreateAmount;
        int value2 = (goldEarned.Value - goldEarned.Value % coinsToCreateAmount) / coinsToCreateAmount;

        while (coinsToCreateAmount > 0)
        {
            GameObject instantiated = Instantiate(goldCoin);
            instantiated.GetComponent<CoinAnimated>().doAnim(begin, goldGui, coinsToCreateAmount == 1 ? value : value2, spawnRadius);

            yield return Yielders.CachedWaitForSeconds(coinSpawnTime);//new WaitForSeconds(coinSpawnTime);
            coinSpawnTime -= coinSpawnAccel;
            if (coinSpawnTime <= 0f)
            {
                coinSpawnTime = 0f;
            }

            coinsToCreateAmount--;
        }

        yield return new WaitForSeconds(endTime);

        finishedCoinEarnCanvas.Raise();
    }

    public void Button_TappedToContinue()
    {
        Debug.Log("Button_TappedToContinue");
        finishedCoinEarnCanvas.Raise();
    }
}
