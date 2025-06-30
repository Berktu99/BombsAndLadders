
using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "string_", menuName = "ScriptableObject/Variables/String")]

public class StringVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public string Value;

    public void SetValue(string value)
    {
        Value = value;
    }

    public void SetValue(StringVariable value)
    {
        Value = value.Value;
    }
}
