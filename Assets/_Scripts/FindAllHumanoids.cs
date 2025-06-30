using UnityEngine;

public class FindAllHumanoids : MonoBehaviour
{
    private Humanoid[] allHumanoids;
    private void Awake()
    {        
        allHumanoids = FindObjectsOfType<Humanoid>(true);

        GetComponent<LevelManager>().humanoidArray = allHumanoids;

        Destroy(this, 2f);
    }
}
