using UnityEngine.Events;

[System.Serializable]
public class UnitySaveStateChangeEvent : UnityEvent<SaveStateChange>
{
    
}

public class SaveStateChange
{
    public SaveState.SaveStateChangeableVariables saveStateChangeable;

    public int newIntValue;
    public bool newBoolValue;
    public string newStringValue;

    public SaveStateChange(int valueToADD, SaveState.SaveStateChangeableVariables saveStateChangeable)
    {
        this.saveStateChangeable = saveStateChangeable;
        this.newIntValue = valueToADD;
    }

    public SaveStateChange(SaveState.SaveStateChangeableVariables saveStateChangeable, int newValue)
    {
        this.saveStateChangeable = saveStateChangeable;
        this.newIntValue = newValue;
    }

    public SaveStateChange(SaveState.SaveStateChangeableVariables saveStateChangeable, bool newValue)
    {
        this.saveStateChangeable = saveStateChangeable;
        this.newBoolValue = newValue;
    }

    public SaveStateChange(SaveState.SaveStateChangeableVariables saveStateChangeable, string newValue)
    {
        this.saveStateChangeable = saveStateChangeable;
        this.newStringValue = newValue;
    }
}