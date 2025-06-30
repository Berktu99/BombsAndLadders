using UnityEngine;

public class UIGoldAmount : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI goldAmountText;
    [SerializeField] private IntVariable goldAmount;

    private int coin = 0;

    private void OnEnable()
    {
        coin = goldAmount.Value;
        goldAmountText.text = goldAmount.Value.ToString();
    }
    
    public void Event_updateGoldText()
    {
        coin = goldAmount.Value;
        goldAmountText.text = goldAmount.Value.ToString();
    }

    public void Event_updateGoldText_TweenEnd(int a)
    {
        coin += a;
        goldAmountText.text = coin.ToString();        
    }
}
