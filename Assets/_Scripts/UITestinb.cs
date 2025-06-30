using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestinb : MonoBehaviour
{    
    public VoidEvent onGoldAmountChanged;
    public VoidEvent stopGameLogic;
    public VoidEvent startGameLogic;
    public VoidEvent resumeGameLogic;
    public VoidEvent onPlayerWon;
    public VoidEvent onPlayerEliminated;
    public VoidEvent onPlayerLost;
    public VoidEvent onShowGoldEarnedPanel;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            onPlayerWon.Raise();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            onPlayerEliminated.Raise();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            onPlayerLost.Raise();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            stopGameLogic.Raise();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            startGameLogic.Raise();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            resumeGameLogic.Raise();
        }

    }
}
