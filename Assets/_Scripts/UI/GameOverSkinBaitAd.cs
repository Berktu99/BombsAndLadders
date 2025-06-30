using UnityEngine;
using UnityEngine.UI;

public class GameOverSkinBaitAd : MonoBehaviour
{
    [MyBox.Foldout("Reward Object stuff", true)]
    [SerializeField] private IntVariable currentRewardPercent;
    [SerializeField] private IntVariable rewardPercentPerGame;

    private int originalRewardPercent;
    private int updatedRewardPercent;

    private System.Collections.Generic.List<int> indexes;

    [SerializeField] private UserInterfaceSubMenus_Showcases showcases; 

    [MyBox.Foldout("Scriptable Object stuff", true)]
    [SerializeField] private AllSkins allSkins;
    [SerializeField] private SaveStateObject saveStateObject;    

    [MyBox.Foldout("Image stuff", true)]
    [SerializeField] private GameObject imagesPanel;
    [SerializeField] private Image fg;
    [SerializeField] private float fgFillSpeed = 1.5f;

    [MyBox.Foldout("Texts stuff", true)]
    [SerializeField] private GameObject textsPanel;
    [SerializeField] private TMPro.TextMeshProUGUI percentText;

    [MyBox.Foldout("Button stuff", true)]
    [SerializeField] private PlusRewardButton button;
    private Coroutine old;

    [MyBox.Foldout("Scriptable Events stuff", true)]
    [SerializeField] private VoidEvent _onBaitCanvasIsFinished;
    [SerializeField] private SaveStateChangeEvent saveStateChangeEvent;
    [SerializeField] private VoidEvent _onPlusRewardButton;
    

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
        getRewardableIndexes();

        if (indexes.Count < 0)
        {
            imagesPanel.SetActive(false);
            textsPanel.SetActive(false);
            button.gameObject.SetActive(false);

            _onBaitCanvasIsFinished.Raise();

            this.gameObject.SetActive(false);
        }
        else
        {
            originalRewardPercent = currentRewardPercent.Value;

            saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.rewardPercent, currentRewardPercent.Value + rewardPercentPerGame.Value));

            updatedRewardPercent = currentRewardPercent.Value;

            imagesPanel.SetActive(true);
            fg.gameObject.SetActive(true);
            fg.fillAmount = currentRewardPercent.Value * 0.01f;

            textsPanel.SetActive(true);
            percentText.text = "%" + 0.ToString();

            old = StartCoroutine(fillReward());

            if (!(currentRewardPercent.Value + rewardPercentPerGame.Value >= 100))
            {
                button.gameObject.SetActive(true);
            }
        }
    }

    private void getRewardableIndexes()
    {
        indexes = new System.Collections.Generic.List<int>();

        for (int i = 0; i < allSkins.characterSkins.availableSkins.Count; i++)
        {
            if(!saveStateObject.isUnlocked(SaveState.Unlockable.CharacterSkins, i))
            {
                indexes.Add(i);
            }            
        }
    }

    public void Event_OnAdWatched() 
    {
        button.GetComponent<Button>().interactable = false;

        updatedRewardPercent = currentRewardPercent.Value;
        StartCoroutine(fillReward());
    }

    private System.Collections.IEnumerator fillReward()
    {
        float fill = originalRewardPercent;
        float targetFill = updatedRewardPercent;

        while (true)
        {
            fill += fgFillSpeed * Time.unscaledDeltaTime;

            if (fill >= 100)
            {
                fill = 100f;
                percentText.text = "%" + Mathf.FloorToInt(fill).ToString();
                fg.fillAmount = 1f;

                break;
            }

            if (fill >= targetFill)
            {
                fill = targetFill;
                percentText.text = "%" + Mathf.FloorToInt(fill).ToString();
                fg.fillAmount = fill * 0.01f;
                break;
            }

            percentText.text = "%" + Mathf.FloorToInt(fill).ToString();
            fg.fillAmount = fill * 0.01f;

            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }

        percentText.text = "%" + fill.ToString();

        if (targetFill >= 100)
        {
            rewardPlayerWithFreeSkin();
            yield break;
        }

        yield return Yielders.CachedWaitForSeconds(1.5f);
        _onBaitCanvasIsFinished.Raise();
    }

    private void rewardPlayerWithFreeSkin()
    {
        int randIndex = indexes[UnityEngine.Random.Range(0, indexes.Count)];
        Unlocker.getInstance().unlockStuff(SaveState.Unlockable.CharacterSkins, randIndex, 0);

        imagesPanel.SetActive(false);
        button.gameObject.SetActive(false);

        StartCoroutine(wait());
        System.Collections.IEnumerator wait()
        {
            yield return new WaitForSeconds(0.2f);
            showcases.gameObject.SetActive(true);
            showcases.equipHumanoidMannequin(UserInterfaceSubMenus_Showcases.SCcameraType.HumanCloseUp);
            showcases.doFireworks();

            yield return Yielders.CachedWaitForSeconds(1.5f);

            _onBaitCanvasIsFinished.Raise();
        }        
    }

    public void Button_PlusReward()
    {
        StopCoroutine(old);
        percentText.text = "%" + Mathf.FloorToInt(updatedRewardPercent).ToString();
        fg.fillAmount = updatedRewardPercent * 0.01f;

        originalRewardPercent = updatedRewardPercent;

        _onPlusRewardButton.Raise();
    }

    #region discard
    //public void Event_OnAdWatched(int rewardAmount)
    //{
    //    //button.gameObject.SetActive(false);

    //    StopCoroutine(old);
    //    StartCoroutine(fillReward(rewardAmount));
    //}

    //private System.Collections.IEnumerator fillReward(int amount)
    //{
    //    float targetFill = currentFill + amount;

    //    while (true)
    //    {
    //        currentFill += fgFillSpeed * Time.unscaledDeltaTime;

    //        if (currentFill >= 100)
    //        {
    //            currentFill = 100f;
    //            percentText.text = "%" + Mathf.FloorToInt(currentFill).ToString();
    //            fg.fillAmount = 1f;

    //            break;
    //        }

    //        if (currentFill >= targetFill)
    //        {
    //            currentFill = targetFill;
    //            percentText.text = "%" + Mathf.FloorToInt(currentFill).ToString();
    //            fg.fillAmount = currentFill * 0.01f;
    //            break;
    //        }

    //        percentText.text = "%" + Mathf.FloorToInt(currentFill).ToString();
    //        fg.fillAmount = currentFill * 0.01f;

    //        yield return new WaitForSeconds(Time.unscaledDeltaTime);
    //    }

    //    percentText.text = "%" + currentFill.ToString();

    //    if (targetFill >= 100)
    //    {
    //        rewardPlayerWithFreeSkin();
    //    }
    //} 
    #endregion
}
