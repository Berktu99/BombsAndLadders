using UnityEngine;

public class SkipLevelButton : MonoBehaviour
{
    [SerializeField] private VoidEvent skipLevelButton;
    public void Button_AdSkipLevel()
    {
        Debug.Log("continu level button is raised but it is not wired up in anywehere to do shit.");
        skipLevelButton.Raise();

        this.gameObject.SetActive(false);
    }
}
