using System;
using UnityEngine;

public class BombSpawner: MonoBehaviour
{
    //[MyBox.Foldout("Debug")]
    //[SerializeField] private float countdownTimerr = 12f;

    [MyBox.Foldout("Variables", true)]
    [SerializeField] private float countdownTime = 12f;
    [SerializeField] private float pickUpTimeShave = 0.04f;
    [SerializeField] private float ladderTimeShave = 0.08f;

    private Pedestal[] pedestals;

    private CountdownTimer countdownTimer;
    private GameObject spawned;
    private int pedestalIndex = -1;

    private bool canSpawn = false;

    private void Awake()
    {
        pedestals = new Pedestal[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            pedestals[i] = transform.GetChild(i).GetComponent<Pedestal>();
        }

        countdownTimer = new CountdownTimer(countdownTime);
    }

    private void Update()
    {
        if (!canSpawn)
            return;

        //countdownTimerr = countdownTimer.timer;
        if (pedestalIsEmpty())
        {
            if (countdownTimer.Tick(Time.unscaledDeltaTime))
            {
                Spawn();                
            }
        }        
    }
    
    public void Event_PickUpPickedUp()
    {
        if (!canSpawn)
            return;

        if (pedestalIsEmpty())
        {
            if (countdownTimer.Tick(pickUpTimeShave))
            {
                Spawn();
            }
        }
    }

    public void Event_LadderPlaced()
    {
        if (!canSpawn)
            return;

        if (pedestalIsEmpty())
        {
            if (countdownTimer.Tick(ladderTimeShave))
            {
                Spawn();
            }
        }
    }

    private bool pedestalIsEmpty()
    {
        for (int i = 0; i < pedestals.Length; i++)
        {
            if (pedestals[i].isEmpty)
            {
                pedestalIndex = i;
                return true;
            }            
        }
        return false;
    }

    private void Spawn()
    {
        spawned = ObjectPooler.getInstance().GetPooledObject(PoolableItems.Bomb);
        spawned.SetActive(true);
        spawned.transform.SetPositionAndRotation(pedestals[pedestalIndex].transform.position, Quaternion.identity);
        spawned.GetComponent<Bomb>().enabled = true;

        pedestals[pedestalIndex].isEmpty = false;
        pedestals[pedestalIndex].bomb = spawned.GetComponent<Bomb>();
    }

    public void EnableBombSpawn()
    {
        canSpawn = true;
    }

    public void DisableBombSpawn()
    {
        canSpawn = false;
    }
}
