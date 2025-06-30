using UnityEngine;

public class BombSpawnManager : MonoBehaviour
{
    private Humanoid[] allHumanoids;
    
    private BombSpawner[] bombSpawner;

    [SerializeField] private bool disableBombSpawn;

    private void Start()
    {
        allHumanoids = FindObjectsOfType<Humanoid>(true);

        bombSpawner = new BombSpawner[transform.childCount];
        for (int i = 0; i < bombSpawner.Length; i++)
        {
            bombSpawner[i] = transform.GetChild(i).GetComponent<BombSpawner>();
        }

        Event_HumanoidChangedFloor();
    }

    public void Event_HumanoidChangedFloor()
    {
        if (disableBombSpawn)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                bombSpawner[i].DisableBombSpawn();
            }
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            bombSpawner[i].DisableBombSpawn();
        }

        for (int i = 0; i < allHumanoids.Length; i++)
        {
            bombSpawner[allHumanoids[i].CurrentFloor].EnableBombSpawn();
        }
    }
}
