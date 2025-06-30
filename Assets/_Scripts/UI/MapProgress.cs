using DG.Tweening;
using UnityEngine;

public class MapProgress : MonoBehaviour
{
    [MyBox.Foldout("SO Dependencies", true)]
    [SerializeField] private IntVariable mapClearRequirement;
    private int _activeMapIndex;

    [SerializeField] private IntVariable _mapsUnlocked_NotUpdated;
    [SerializeField] private IntVariable notUpdated_currentMapCleared;
    [SerializeField] private IntVariable updated_currentMapCleared;    

    [SerializeField] private MapsSO maps;

    [MyBox.Foldout("Self Fields", true)]
    [SerializeField] private UnityEngine.UI.Image[] progressBars;
    [SerializeField] private UnityEngine.UI.Image map1, map2;
    [SerializeField] private Transform text;

    [MyBox.Foldout("Animation Fields", true)]
    [SerializeField] private float progressBarSpeed = 0.8f;
    private float currentFill = 0f;

    [SerializeField] private float textTweenTime = 0.3f;

    private void OnEnable()
    {
        checkIfLastMapAvailable();          
    }

    private void checkIfLastMapAvailable()
    {
        _activeMapIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
        int lastUnlockedMapIndex = 0;

        while (true)
        {
            if ((_mapsUnlocked_NotUpdated.Value & (1 << (lastUnlockedMapIndex))) == 0)
            {
                lastUnlockedMapIndex--;
                break;
            }
            else
            {
                lastUnlockedMapIndex++;
            }
        }

        if (_activeMapIndex == lastUnlockedMapIndex)
        {
            if ( (maps.availableMaps.Count-1 > lastUnlockedMapIndex))
            {                
                PrepPanel();
                StartCoroutine(CoRoutine_AnimateProgress());
                return;
            }
        }

        // Cant unlock next map
        gameObject.SetActive(false);
    }

    private void PrepPanel()
    {
        text.localScale = Vector3.zero;

        map1.sprite = maps.availableMaps[_activeMapIndex].sprite;
        map2.sprite = maps.availableMaps[_activeMapIndex + 1].sprite;

        for (int i = 0; i < progressBars.Length; i++)
        {
            progressBars[i].fillAmount = 0f;
        }

        for (int i = 0; i < notUpdated_currentMapCleared.Value; i++)
        {
            progressBars[i].fillAmount = 1f;
        }
    }

    private System.Collections.IEnumerator CoRoutine_AnimateProgress()
    {
        if (notUpdated_currentMapCleared.Value >= updated_currentMapCleared.Value)
            yield break;
        
        while (true)
        {
            currentFill += progressBarSpeed * Time.unscaledDeltaTime;
            progressBars[updated_currentMapCleared.Value - 1].fillAmount = currentFill;
            yield return Yielders.CachedWaitForSeconds(Time.unscaledDeltaTime);

            if (progressBars[updated_currentMapCleared.Value - 1].fillAmount >= 1f)
            {
                progressBars[updated_currentMapCleared.Value - 1].fillAmount = 1f;
                break;
            }
        }

        if (updated_currentMapCleared.Value >= mapClearRequirement.Value)
        {
            text.DOScale(Vector3.one, textTweenTime).SetEase(Ease.InOutBounce);
        }
    }
}
