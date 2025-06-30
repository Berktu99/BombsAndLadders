using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SceneHandler _sceneHandler;

    [SerializeField] private IntVariable goldEarned;
    [SerializeField] private VoidEvent OnStartGame_ObjPooler;

    [MyBox.Foldout("UI MANAGER EVENTS", true)]
    [SerializeField] private VoidEvent OnStartGame_UIManager;
    [SerializeField] private VoidEvent OnPauseGame_UIManager;
    [SerializeField] private VoidEvent OnResumeGame_UIManager;
    [SerializeField] private VoidEvent OnStopGame_UIManager;
    [SerializeField] private VoidEvent OnPlayerWon_UIManager;
    [SerializeField] private VoidEvent OnPlayerLost_UIManager;
    [SerializeField] private VoidEvent OnPlayerEliminated_UIManager;

    [MyBox.Foldout("LEVEL MANAGER EVENTS", true)]
    [SerializeField] private VoidEvent OnStartGame_LevelManager;
    [SerializeField] private VoidEvent OnResumeGame_LevelManager;
    [SerializeField] private VoidEvent OnStopGame_LevelManager;
    [SerializeField] private VoidEvent OnPlayerWon_LevelManager;
    [SerializeField] private VoidEvent OnPlayerLost_LevelManager;
    [SerializeField] private VoidEvent OnPlayerEliminated_LevelManager;
    

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;

        goldEarned.SetValue(0);
    }

    private void Start()
    {
        stopGameLogic();
    }

    public void Event_OnTapToPlay()
    {
        OnStartGame_LevelManager.Raise();
        OnStartGame_UIManager.Raise();

        OnStartGame_ObjPooler.Raise();
    }

    public void Event_OnPlayerWonRace()
    {
        manageGoldEarnEconomy();

        _sceneHandler.UpdateMap();
        OnPlayerWon_LevelManager.Raise();
        OnPlayerWon_UIManager.Raise();
    }
    

    public void Event_OnPlayerLostRace()
    {
        manageGoldEarnEconomy();

        OnPlayerLost_LevelManager.Raise();
        OnPlayerLost_UIManager.Raise();
    }

    public void Event_OnPlayerEliminated()
    {
        manageGoldEarnEconomy();

        OnPlayerEliminated_LevelManager.Raise();
        OnPlayerEliminated_UIManager.Raise();
    }


    private void manageGoldEarnEconomy()
    {
        if (goldEarned.Value < 5)
        {
            goldEarned.SetValue(15);
        }
        else if (goldEarned.Value < 10)
        {
            goldEarned.SetValue(20);
        }
        else if (goldEarned.Value < 15)
        {
            goldEarned.SetValue(25);
        }
        else if (goldEarned.Value < 20)
        {
            goldEarned.SetValue(35);
        }
        else if (goldEarned.Value < 25)
        {
            goldEarned.SetValue(50);
        }
        else if (goldEarned.Value < 30)
        {
            goldEarned.SetValue(75);
        }
        else if (goldEarned.Value < 40)
        {
            goldEarned.SetValue(100);
        }
        else if (goldEarned.Value < 50)
        {
            goldEarned.SetValue(120);
        }
        else
        {
            goldEarned.SetValue(150);
        }
    }
    public void Event_OnContinueLevelAdWatched()
    {
        OnResumeGame_UIManager.Raise();
        OnResumeGame_LevelManager.Raise();
    }

    public void Event_OnLoadNextScene()
    {
        _sceneHandler.LoadScene();
    }

    public void stopGameLogic()
    {
        OnStopGame_UIManager.Raise();
        OnStopGame_LevelManager.Raise();
    }
}
