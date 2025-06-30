using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueLevelButton : MonoBehaviour
{

    [SerializeField] private VoidEvent continueLevelButton;
    public void Button_ContinueLevelAd()
    {
        Debug.Log("continu level button is raised but it is not wired up in anywehere to do shit.");
        continueLevelButton.Raise();
    }
}
