using UnityEngine;

[System.Serializable]
public class LoseByFallPanel
{
    public enum Panels
    {
        Parent,
        Pre,
        After,
        AllOfIt,
    }

    public GameObject loseByFallPanelsParent;
    public GameObject preLoseByFallPanel;
    public GameObject afterLoseByFallPanel;

    public void showPanel(bool show, Panels which)
    {
        switch (which)
        {
            case Panels.Parent:
                {
                    loseByFallPanelsParent.SetActive(show);
                    preLoseByFallPanel.SetActive(!show);
                    afterLoseByFallPanel.SetActive(!show);
                    break;
                }
            case Panels.Pre:
                {
                    loseByFallPanelsParent.SetActive(true);
                    preLoseByFallPanel.SetActive(show);
                    afterLoseByFallPanel.SetActive(!show);
                    break;
                }
            case Panels.After:
                {
                    loseByFallPanelsParent.SetActive(true);
                    preLoseByFallPanel.SetActive(!show);
                    afterLoseByFallPanel.SetActive(show);
                    break;
                }
            case Panels.AllOfIt:
                {
                    loseByFallPanelsParent.SetActive(show);
                    preLoseByFallPanel.SetActive(show);
                    afterLoseByFallPanel.SetActive(show);
                    break;
                }
            default:
                {
                    Debug.Log("You did not account for: " + which);
                    break;
                }
        }
    }
}

public class GameOverEliminated : MonoBehaviour
{    
    [SerializeField] private LoseByFallPanel panels;

    [SerializeField] private UIGoldAmount coinAmount;

    [SerializeField] private UnityEngine.UI.Button isEnoughButton;
    [SerializeField] private float waitTime = 4f;

    private void Awake()
    {
        coinAmount.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        panels.preLoseByFallPanel.SetActive(true);      
        panels.afterLoseByFallPanel.SetActive(false);
    }
    public void Button_NoThanks_LoseByFall()
    {
        panels.preLoseByFallPanel.SetActive(false);

        panels.afterLoseByFallPanel.SetActive(true);
        coinAmount.gameObject.SetActive(true);

        StartCoroutine(waitToRevealEnoughText());
    }

    private System.Collections.IEnumerator waitToRevealEnoughText()
    {
        // >:D MUHAHAHAH
        yield return new WaitForSeconds(waitTime);

        isEnoughButton.gameObject.SetActive(true);
    }
}
