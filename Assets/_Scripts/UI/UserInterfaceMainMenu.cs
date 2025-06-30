using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class UserInterfaceMainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;

    [SerializeField] private UserInterfaceSubMenus subMenus;

    private bool shouldCheckTapToPlay = true;

    [SerializeField] private StringVariable playerName;

    [SerializeField] private SaveStateChangeEvent saveStateChangeEvent;

    [SerializeField] private VoidEvent OnTappedToPlay;

    private void OnEnable()
    {
        hideChildren(false);

        nameField.SetTextWithoutNotify(playerName.Value);
    }

    private void Update()
    {
        OnTapToPlay();
    }

    public void Button_Settings()
    {
        subMenus.gameObject.SetActive(true);
        subMenus.hideSelfChildren(false, SubMenuChildren.Settings);
        toggleTapToPlay(false);
    }

    public void Button_Maps()
    {
        subMenus.gameObject.SetActive(true);
        subMenus.hideSelfChildren(false, SubMenuChildren.Maps);
        toggleTapToPlay(false);
    }

    public void Button_Customize()
    {
        subMenus.gameObject.SetActive(true);
        subMenus.hideSelfChildren(false, SubMenuChildren.Customize);

        hideChildren(true);

        toggleTapToPlay(false);
    }

    public void Button_Store()
    {
        subMenus.gameObject.SetActive(true);

        subMenus.hideSelfChildren(false, SubMenuChildren.Store);

        toggleTapToPlay(false);
    }

    public void Button_OnNameChanged(string newName)
    {
        saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.playerName, newName));
    }

    public void toggleTapToPlay(bool t)
    {
        shouldCheckTapToPlay = t;
    }

    private void OnTapToPlay()
    {
        if (Input.GetMouseButtonDown(0) && shouldCheckTapToPlay)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            //Debug.Log("heyo, tapped to play");
            OnTappedToPlay.Raise();
        }
    }

    public void hideChildren(bool yes)
    {
        if (yes)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
