using UnityEngine;

public class MultiplyGold : Singleton<MultiplyGold>
{
    [SerializeField] private IntVariable earnedGold;
    [SerializeField] private IntVariable goldAmount;

    [SerializeField] private SaveStateChangeEvent saveStateChange;

    [SerializeField] private VoidEvent updateGoldAmountText;

    public void OnMultiplyGold(int mult)
    {
        earnedGold.SetValue(earnedGold.Value * mult);

        int oldVal = goldAmount.Value;
        saveStateChange.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.gold, goldAmount.Value + earnedGold.Value));
        Debug.Log("gold amount after the multiply gold: " + goldAmount.Value);
        goldAmount.SetValue(oldVal);
        updateGoldAmountText.Raise();
    }
}
