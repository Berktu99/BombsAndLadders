using UnityEngine;

public class UserInterfaceGameOver : MonoBehaviour
{
    public GameOverWin gameOverWin;
    public GameOverEliminated gameOverEliminated;
    public GameOverLose gameOverLose;
    public UserInterface_CoinEarnCanvas earnCoinCanvas;
    public GameOverSkinBaitAd baitAdCanvas;

    private void Awake()
    {
        gameOverLose.gameObject.SetActive(false);
        gameOverEliminated.gameObject.SetActive(false);
        earnCoinCanvas.gameObject.SetActive(false);
        gameOverWin.gameObject.SetActive(false);
    }

    public void onPlayerWon()
    {
        gameOverLose.gameObject.SetActive(false);
        gameOverEliminated.gameObject.SetActive(false);
        earnCoinCanvas.gameObject.SetActive(false);

        gameOverWin.gameObject.SetActive(true);
    }

    public void onPlayerEliminated()
    {
        gameOverLose.gameObject.SetActive(false);
        gameOverWin.gameObject.SetActive(false);
        earnCoinCanvas.gameObject.SetActive(false);

        gameOverEliminated.gameObject.SetActive(true);
    }
    public void onPlayerLost()
    {
        gameOverWin.gameObject.SetActive(false);
        gameOverEliminated.gameObject.SetActive(false);
        earnCoinCanvas.gameObject.SetActive(false);

        gameOverLose.gameObject.SetActive(true);
    }

    public void showCoinEarnedPanel()
    {
        //gameOverLose.gameObject.SetActive(false);
        //gameOverWin.gameObject.SetActive(false);
        //gameOverEliminated.gameObject.SetActive(false);

        earnCoinCanvas.gameObject.SetActive(true);
    }

    public void showSkinBaitCanvas()
    {
        gameOverLose.gameObject.SetActive(false);
        gameOverWin.gameObject.SetActive(false);
        gameOverEliminated.gameObject.SetActive(false);
        earnCoinCanvas.gameObject.SetActive(false);

        baitAdCanvas.gameObject.SetActive(true);        
    }
}
