using UnityEngine;

public class GameOverLose : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button isEnoughButton;

    [SerializeField] private float waitTime = 4f;
    private void OnEnable()
    {

    }

    private System.Collections.IEnumerator waitToRevealEnoughText()
    {
        // >:D MUHAHAHAH
        yield return new WaitForSeconds(waitTime);

        isEnoughButton.gameObject.SetActive(true);
    }
}
