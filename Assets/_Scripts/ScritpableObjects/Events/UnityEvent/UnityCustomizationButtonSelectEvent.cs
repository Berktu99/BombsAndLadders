using UnityEngine.Events;

[System.Serializable]
public class UnityCustomizationButtonSelectEvent : UnityEvent<CustomizationButtonSelect>
{
    
}

public class CustomizationButtonSelect
{
    public SaveState.Unlockable type;
    public int buttonIndex;

    public CustomizationButtonSelect()
    {
        type = SaveState.Unlockable.Null;
        buttonIndex = -1;
    }
    public CustomizationButtonSelect(int buttonIndex, SaveState.Unlockable type) 
    { 
        this.buttonIndex = buttonIndex;
        this.type = type;
    }
}
