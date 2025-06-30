using UnityEngine;
public enum ActivateUITarget
{
    MainMenu,
    GameOver,
}

public class UserInterfaceManager : MonoBehaviour
{
    [SerializeField] private IntVariable goldEarned;

    public GameObject inGamePanel;
    public UserInterfaceMainMenu mainMenu;
    public UserInterfaceSubMenus subMenus;
    public UserInterfaceGameOver gameOver;
    public UserInterface_CoinEarnCanvas earnCanvas;
    public GameOverSkinBaitAd baitAdCanvas;
    [SerializeField] private UserInterfaceSubMenus_Showcases showcases;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        mainMenu.gameObject.SetActive(false);
        subMenus.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        earnCanvas.gameObject.SetActive(false);
        baitAdCanvas.gameObject.SetActive(false);
        showcases.gameObject.SetActive(false);

        goldEarned.SetValue(0);        
    }    

    public void Event_stopGameLogic()
    {
        stopGameLogic(ActivateUITarget.MainMenu);
    }

    public void Event_startGameLogic()
    {
        inGamePanel.SetActive(true);

        mainMenu.hideChildren(true);

        mainMenu.gameObject.SetActive(false);        

        subMenus.gameObject.SetActive(false);   

        gameOver.gameObject.SetActive(false);

        earnCanvas.gameObject.SetActive(false);
    }

    public void Event_OnPlayerWon()
    {
        stopGameLogic(ActivateUITarget.GameOver);
        gameOver.onPlayerWon();        
    }

    public void Event_OnPlayerLost()
    {
        stopGameLogic(ActivateUITarget.GameOver);
        gameOver.onPlayerLost();
    }

    public void Event_OnPlayerEliminated()
    {
        stopGameLogic(ActivateUITarget.GameOver);
        gameOver.onPlayerEliminated();
    }

    public void Event_ShowGoldEarnedPanel()
    {
        stopGameLogic(ActivateUITarget.GameOver);
        gameOver.showCoinEarnedPanel();
    }

    public void Event_OnShowSkinBaitAd()
    {
        stopGameLogic(ActivateUITarget.GameOver);
        gameOver.showSkinBaitCanvas();
    }

    public void stopGameLogic(ActivateUITarget showUITarget)
    {
        inGamePanel.SetActive(false);

        switch (showUITarget)
        {
            case ActivateUITarget.MainMenu:
                {
                    mainMenu.gameObject.SetActive(true);
                    subMenus.gameObject.SetActive(false);
                    gameOver.gameObject.SetActive(false);
                    break;
                }            
            case ActivateUITarget.GameOver:
                {
                    mainMenu.gameObject.SetActive(false);
                    subMenus.gameObject.SetActive(false);
                    gameOver.gameObject.SetActive(true);
                    break;
                }            
            default:
                {
                    Debug.LogWarning("You did not account for this.");
                    break;
                }
        }
    }
}
