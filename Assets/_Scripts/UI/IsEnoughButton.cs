using UnityEngine;

public class IsEnoughButton : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI isEnoughText;
    [SerializeField] private IntVariable goldEarned;

    [SerializeField] private VoidEvent isEnoughButton;

    private void OnEnable()
    {
        isEnoughText.text = goldEarned.Value.ToString() + " $ is enuogh!";
    }

    public void Button_IsEnough_Won()
    {
        MultiplyGold.getInstance().OnMultiplyGold(1);

        isEnoughButton.Raise();

        GetComponent<UnityEngine.UI.Button>().interactable = false;
        //this.gameObject.SetActive(false);
    }
}
