using UnityEngine;

public class GameOverWin : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;

    [SerializeField] private GameObject doubleGoldParent;
    [SerializeField] private GameObject spinMiniGame;

    [SerializeField] private UIGoldAmount coinAmount;

    [SerializeField] private UnityEngine.UI.Button doubleGoldButton;
    [SerializeField] private UnityEngine.UI.Button isEnoughButton;

    [SerializeField] private float waitTime = 4f;

    private void Awake()
    {
        doubleGoldButton.gameObject.SetActive(false);
        isEnoughButton.gameObject.SetActive(false);

        doubleGoldParent.SetActive(false);
        spinMiniGame.SetActive(false);

        coinAmount.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        winPanel.SetActive(true);
        coinAmount.gameObject.SetActive(true);

        if (Random.Range(0,2) == 0)
        {
            spinMiniGame.SetActive(false);

            doubleGoldParent.SetActive(true);
            doubleGoldButton.gameObject.SetActive(true);
            isEnoughButton.gameObject.SetActive(false);            
        }
        else
        {
            doubleGoldParent.SetActive(false);

            spinMiniGame.SetActive(true);
        }

        StartCoroutine(waitToRevealEnoughText());
    }

    private System.Collections.IEnumerator waitToRevealEnoughText()
    {
        // >:D MUHAHAHAH
        yield return new WaitForSeconds(waitTime);

        isEnoughButton.gameObject.SetActive(true);
    }
}
