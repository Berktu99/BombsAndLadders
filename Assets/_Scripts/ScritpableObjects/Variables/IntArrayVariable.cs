using UnityEngine;

[CreateAssetMenu(fileName = "NewIntArrayVariable", menuName = "ScriptableObject/Variables/IntArray")]

public class IntArrayVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public int[] Values;

    public void clearArray()
    {
        Values = new int[0];
    }

    public void initArray(int length)
    {
        Values = new int[length];
    }

    public void SetNewValues(int[] array)
    {
        initArray(array.Length);

        for (int i = 0; i < array.Length; i++)
        {
            Values[i] = array[i];
        }
    }

    public void SetValueAtIndex(int value, int index)
    {
        Values[index] = value;
    }

    public void SetValueAtIndex(IntVariable value, int index)
    {
        Values[index] = value.Value;
    }

    //public void ApplyChange(int amount)
    //{
    //    Values += amount;
    //}

    //public void ApplyChange(IntVariable amount)
    //{
    //    Values += amount.Value;
    //}
}
