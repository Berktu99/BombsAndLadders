using MyBox;
using UnityEngine;

[System.Serializable]
public class ToggleButton
{
    public UnityEngine.UI.Image buttonBG;

    public Sprite buttonBG_On;
    public Sprite buttonBG_Off;

    public GameObject indicatorObject;
}

public class UserInterfaceSubMenus_Settings : MonoBehaviour
{
    [SerializeField] private UserInterfaceMainMenu mainMenu;
    [SerializeField] private UserInterfaceSubMenus subMenus;

    [Foldout("Buttons", true)]
    public ToggleButton muteToggle;
    public ToggleButton vibrationToggle;

    [Foldout("Scriptable Objects Stuff", true)]
    public SaveStateObject saveStateObject;

    [Separator("SaveStateChange Event", true)]
    public SaveStateChangeEvent saveStateChangeEvent;

    private void OnEnable()
    {
        initializeSubMenuScreen();        
    }

    void initializeSubMenuScreen()
    {
        manageVibrateToggle();
        void manageVibrateToggle()
        {
            if (saveStateObject.isVibrateToggledOn.Value == true)
            {
                vibrationToggle.indicatorObject.SetActive(true);
                vibrationToggle.buttonBG.sprite = vibrationToggle.buttonBG_On;
            }
            else
            {
                vibrationToggle.indicatorObject.SetActive(false);
                vibrationToggle.buttonBG.sprite = vibrationToggle.buttonBG_Off;
            }
        }

        manageMuteToggle();
        void manageMuteToggle()
        {
            if (saveStateObject.isMuteToggledOn.Value == true)
            {
                muteToggle.indicatorObject.SetActive(true);
                muteToggle.buttonBG.sprite = muteToggle.buttonBG_On;
            }
            else
            {
                muteToggle.indicatorObject.SetActive(false);
                muteToggle.buttonBG.sprite = muteToggle.buttonBG_Off;
            }
        }
    }

    public void Button_OnVibrationToggle()
    {
        saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.isVibrateToggledOn, !saveStateObject.isVibrateToggledOn.Value));

        vibrationToggle.indicatorObject.SetActive(saveStateObject.isVibrateToggledOn.Value);

        if (saveStateObject.isVibrateToggledOn.Value == false)
        {            
            vibrationToggle.buttonBG.sprite = vibrationToggle.buttonBG_On;
        }
        else
        {
            vibrationToggle.buttonBG.sprite = vibrationToggle.buttonBG_Off;
        }
    }

    public void Button_OnMuteToggle()
    {
        saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.isMuteToggledOn, !saveStateObject.isMuteToggledOn.Value));

        muteToggle.indicatorObject.SetActive(saveStateObject.isMuteToggledOn.Value);

        if (saveStateObject.isMuteToggledOn.Value == false)
        {
            muteToggle.buttonBG.sprite = muteToggle.buttonBG_On;
        }
        else
        {
            muteToggle.buttonBG.sprite = muteToggle.buttonBG_Off;
        }
    }

    public void Button_OnCloseSettingsPanel()
    {
        mainMenu.hideChildren(false);
        mainMenu.toggleTapToPlay(true);

        this.gameObject.SetActive(false);
        subMenus.gameObject.SetActive(false);
    }
}
