using UnityEngine;

public class SkipLevel : MonoBehaviour
{
    [SerializeField] private SaveStateChangeEvent _saveStateChange;

    [SerializeField] private IntVariable _mapClearRequirement;
    public void SkipThisLevel()
    {
        Debug.Log("skip levle pls");
        _saveStateChange.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.allMapsClearedArray, _mapClearRequirement));
    }
}
