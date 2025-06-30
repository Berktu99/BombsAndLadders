using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    //private static readonly string SplashScene = "Splash";
    private static readonly string LoadingScene = "Loading";

    //public enum Maps
    //{
    //    Level_0,
    //    Level_1,
    //    Level_2,
    //    Level_3,
    //    Level_4,
    //    Level_5,
    //    Level_6,
    //    Level_7,
    //    Level_8,
    //    Level_9,
    //    Level_10,
    //    Level_11,       
    //}

    public static System.Action OnLoaderCallback;

    private class LoadingMono : MonoBehaviour { }
    private static AsyncOperation loadingAsyncOperation;
    public static void loaderCallBack()
    {
        if (OnLoaderCallback != null)
        {
            OnLoaderCallback();
            OnLoaderCallback = null;
        }
    }

    public static void loadMap(int targetIndex)
    {
        OnLoaderCallback = () =>
        {
            //SceneManager.LoadScene(targetScene.ToString());
            //return;
            GameObject loadingGameObject = new GameObject("LoadingGameObject");
            loadingGameObject.AddComponent<LoadingMono>().StartCoroutine(LoadMapAsync(targetIndex));
        };

        SceneManager.LoadScene(LoadingScene);
    }

    //public static void loadScene(Maps targetScene)
    //{
    //    OnLoaderCallback = () =>
    //    {
    //        GameObject loadingGameObject = new GameObject("LoadingGameObject");
    //        loadingGameObject.AddComponent<LoadingMono>().StartCoroutine(LoadSceneAsync(targetScene));   
    //    };

    //    SceneManager.LoadScene(LoadingScene);
    //}

    //private static IEnumerator LoadSceneAsync(Maps targetScene)
    //{
    //    yield return null;

    //    loadingAsyncOperation = SceneManager.LoadSceneAsync(targetScene.ToString());

    //    while(!loadingAsyncOperation.isDone)
    //    {
    //        yield return null;
    //    }
    //}

    private static IEnumerator LoadMapAsync(int index)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync("Level_" + index.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float getLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
}
