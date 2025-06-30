using UnityEngine;

public class MultiplyGoldButton : MonoBehaviour
{
    [SerializeField] private int multAmount = 2;

    [SerializeField] private IntEvent multiplyGold;    
    [SerializeField] private IntVariable goldEarned;

    [SerializeField] private TMPro.TextMeshProUGUI multGoldText;

    private void OnEnable()
    {
        multGoldText.text = (goldEarned.Value * 2).ToString();
    }

    public void Button_MultGoldAd()
    {
        multiplyGold.Raise(multAmount);

        this.gameObject.SetActive(false);
    }
    
}
