using UnityEngine;

public class SplashToGameScene : MonoBehaviour
{
    [SerializeField] private float _splashScreenShowTime = 2f;

    [SerializeField] private IntVariable _mapActiveIndex;

    private void Awake()
    {
        StartCoroutine(Co_SplashScreen());
    }

    private System.Collections.IEnumerator Co_SplashScreen()
    {
        yield return new WaitForSeconds(_splashScreenShowTime);
        
        Loader.loadMap(_mapActiveIndex.Value);
    }
}
