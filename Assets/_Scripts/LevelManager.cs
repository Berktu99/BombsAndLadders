using System.Collections.Generic;
using UnityEngine;

public class UnoccupiedCellIndexAtFloor
{
    public List<int> unoccupiedCellIndexes;
    public UnoccupiedCellIndexAtFloor()
    {
        unoccupiedCellIndexes = new List<int>();
    }
}
public class SingleHumanoidRepooledPickUps
{
    public List<PickUp> repooledPickUps;

    public SingleHumanoidRepooledPickUps()
    {
        repooledPickUps = new List<PickUp>();
    }
}

[System.Serializable]
public class Cell
{
    public Vector3 position;
    public Quaternion rotation;
    public bool occupied;

    public Cell(Vector3 pos, Quaternion rot,bool occupancy)
    {
        position = pos;
        rotation = rot;
        occupied = occupancy;
    }
}
[System.Serializable]
public class FloorPickUp
{
    public Cell[] cells;

    public FloorPickUp(int cellCount)
    {
        cells = new Cell[cellCount];
    }
}

public class LevelManager : Singleton<LevelManager>
{
    [HideInInspector] public Humanoid[] humanoidArray;
    private Humanoid[] aiArray;
    private Humanoid player;

    private int floorCount;
    public int FloorCount { get { return floorCount; } }

    private PickUpsPooler pickUpsPooler;

    // Last floor is podium for winner poses and stuff
    private FloorPickUp[] floors;

    public Transform podium;
    public Cinemachine.CinemachineVirtualCamera podiumCamera;

    public SingleHumanoidRepooledPickUps[] allHumanoidRepooledPickUps;
    private UnoccupiedCellIndexAtFloor[] allFloorsUnoccupiedCellIndexes;

    private int[] aiHumanoidColorSkinIndexes;

    private int[] pickUpCountPerHumanoidPerFloor;

    private BombSpawnManager bombSpawnManager;

    private Transform platformsParent;
    [HideInInspector] public int[] ladderCountBetweenFloors;

    private int humanoidCount = 0;

    private int[] keyToFloor;

    private bool canSpawnPickUps = true;

    private Humanoid winner = null;

    public HumanoidSkins humanoidSkins;
    public ColorSkins colorSkins;

    public IntVariable colorSkinActiveIndex;

    public VoidEvent OnPlayerWonRace;
    public VoidEvent OnPlayerLostRace;

    private SaveManager saveManager;

    protected override void Awake()
    {
        base.Awake();
        bombSpawnManager = GetComponentInChildren<BombSpawnManager>();
        bombSpawnManager.gameObject.SetActive(false);
    }

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
        saveManager.saveState._mapActiveIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
        saveManager.saveStateObject._mapActiveIndex.SetValue(saveManager.saveState._mapActiveIndex);

        pickUpsPooler = FindObjectOfType<PickUpsPooler>();
        

        initListVariables();
        void initListVariables()
        {
            humanoidCount = humanoidArray.Length;

            keyToFloor = new int[humanoidCount];
            for (int i = 0; i < keyToFloor.Length; i++)
            {
                // keyFloor is a list indicating
                // every humanoid's current floor i think
                // all of them starts at 0th floor
                keyToFloor[i] = 0;
            }

            allHumanoidRepooledPickUps = new SingleHumanoidRepooledPickUps[humanoidCount];
            for (int i = 0; i < allHumanoidRepooledPickUps.Length; i++)
            {
                allHumanoidRepooledPickUps[i] = new SingleHumanoidRepooledPickUps();
            }


            floorCount = transform.GetChild(3).childCount;
            floors = new FloorPickUp[floorCount - 1];
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i] = new FloorPickUp(transform.GetChild(0).GetChild(i).childCount);
            }


            allFloorsUnoccupiedCellIndexes = new UnoccupiedCellIndexAtFloor[floorCount];
            for (int i = 0; i < allFloorsUnoccupiedCellIndexes.Length; i++)
            {
                allFloorsUnoccupiedCellIndexes[i] = new UnoccupiedCellIndexAtFloor();
            }


            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                int cellCount = transform.GetChild(0).GetChild(i).childCount;

                for (int j = 0; j < cellCount; j++)
                {
                    allFloorsUnoccupiedCellIndexes[i].unoccupiedCellIndexes.Add(j);

                    floors[i].cells[j] = new Cell(transform.GetChild(0).GetChild(i).GetChild(j).position, transform.GetChild(0).GetChild(i).GetChild(j).rotation, false);
                }
                ShuffleExtension.Shuffle(floors[i].cells, cellCount);
            }

            aiArray = new Humanoid[humanoidCount - 1];
            int z = 0;
            for (int i = 0; i < humanoidCount; i++)
            {
                if (humanoidArray[i].CompareTag("ai"))
                {
                    aiArray[z] = humanoidArray[i];
                    z++;
                }
                else
                {
                    player = humanoidArray[i];
                }
            }

            for (int i = 0; i < humanoidArray.Length; i++)
            {
                humanoidArray[i].initHumanoidVariables(i,this);
            }
        }

        checkForErrors();
        void checkForErrors()
        {
            Debug.Log("implement check for errors pls.");
        }

        calculatePickUpCounts();
        void calculatePickUpCounts()
        {
            pickUpCountPerHumanoidPerFloor = new int[floorCount - 1];

            int cellCount = floors[0].cells.Length;

            int perChar = cellCount / (humanoidCount);

            pickUpCountPerHumanoidPerFloor[0] = perChar;

            for (int i = 1; i < floorCount - 1; i++)
            {
                cellCount = floors[i].cells.Length;
                perChar = cellCount / (humanoidCount);
                pickUpCountPerHumanoidPerFloor[i] = perChar;
            }
            
        }

        calculateLadderCounts();
        void calculateLadderCounts()
        {
            platformsParent = transform.GetChild(3);            

            // ladder height is 1, so it is very convenient
            ladderCountBetweenFloors = new int[floorCount - 1];
            float ladderHeight = 1f;//ObjectPooler.getInstance().GetPooledObject(PoolableItems.LadderGFX).transform.GetChild(0).lossyScale.y;

            for (int i = 0; i < ladderCountBetweenFloors.Length; i++)
            {
                ladderCountBetweenFloors[i] = Mathf.CeilToInt( (platformsParent.transform.GetChild(i + 1).GetChild(0).position.y - platformsParent.transform.GetChild(i).GetChild(0).position.y) / ladderHeight);
                //Debug.Log("ladder count: + " + ladderCountBetweenFloors[i]);
            }
        }      

        handleAICustomization();
    }
    
    private void handleAICustomization()
    {
        int randomSkin;
        handleNameAssignment();
        void handleNameAssignment()
        {
            List<string> names = NameGenerator.GetRandomNames(aiArray.Length);
            for (int i = 0; i < aiArray.Length; i++)
            {
                aiArray[i].assignName(names[i]);
            }
        }

        handleSkinAssignment();
        void handleSkinAssignment()
        {
            for (int i = 0; i < aiArray.Length; i++)
            {
                randomSkin = Random.Range(0, humanoidSkins.availableSkins.Count);
                aiArray[i].equipNewCharacterSkin(humanoidSkins.availableSkins[randomSkin].skinType, humanoidSkins.availableSkins[randomSkin].skinPrefab); 
            }
        }

        Event_handleHumanoidColorSkin();
    }
    
    public void Event_handleHumanoidColorSkin()
    {
        // Humanoid characters are filthy rich, all skins are unlocked for them.            
        int playerActiveIndex = colorSkinActiveIndex.Value;
        
        if (aiHumanoidColorSkinIndexes == null)
        {
            aiHumanoidColorSkinIndexes = new int[aiArray.Length];

            assignFirstTime();
            void assignFirstTime()
            {
                int humanoidIndex;

                while (true)
                {
                    humanoidIndex = Random.Range(0, colorSkins.availableSkins.Count);
                    if (humanoidIndex != playerActiveIndex)
                    {
                        aiHumanoidColorSkinIndexes[0] = humanoidIndex;
                        break;
                    }
                }


                for (int i = 1; i < aiHumanoidColorSkinIndexes.Length;)
                {
                    humanoidIndex = Random.Range(0, colorSkins.availableSkins.Count);
                    bool same = false;

                    if (humanoidIndex != playerActiveIndex)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (humanoidIndex == aiHumanoidColorSkinIndexes[j])
                            {
                                same = true;
                                break;
                            }
                        }
                        if (!same)
                        {
                            aiHumanoidColorSkinIndexes[i] = humanoidIndex;
                            i++;
                        }                        
                    }                    
                }


                for (int i = 0; i < aiArray.Length; i++)
                {
                    //Material material = Instantiate(humanoidMaterial);
                    aiArray[i].equipNewColorSkin(colorSkins.availableSkins[aiHumanoidColorSkinIndexes[i]].skinColor);
                }
            }
        }
        else
        {            
            // Piþti olduk mu ona bir bak.
            for (int i = 0; i < aiHumanoidColorSkinIndexes.Length; i++)
            {                
                if (playerActiveIndex == aiHumanoidColorSkinIndexes[i])
                {
                    int newIndex = Random.Range(0, colorSkins.availableSkins.Count);

                    for (int j = 0; j < aiHumanoidColorSkinIndexes.Length; j++)
                    {
                        if (newIndex == aiHumanoidColorSkinIndexes[j] || newIndex == playerActiveIndex)
                        {
                            newIndex = Random.Range(0, colorSkins.availableSkins.Count);
                            j = 0;
                        }
                    }

                    aiHumanoidColorSkinIndexes[i] = newIndex;

                    aiArray[i].equipNewColorSkin(colorSkins.availableSkins[newIndex].skinColor);

                    break;
                }
            }
        }
    }

    public void Event_StopGameLogic()
    {
        foreach (Humanoid humanoid in humanoidArray)
        {
            humanoid.stopHumanoidMovement(false);
        }

        bombSpawnManager.gameObject.SetActive(false);

        canSpawnPickUps = false;
    }

    public void Event_StartGameLogic()
    {
        pickUpsPooler.makePool(1, humanoidArray);

        foreach (Humanoid humanoid in humanoidArray)
        {
            humanoid.startHumanoidMovement();
        }

        bombSpawnManager.gameObject.SetActive(true);

        canSpawnPickUps = true;

        spawnFloorSingle(0);
        spawnFloorSingle(1);
        spawnFloorSingle(2);
        spawnFloorSingle(3);
    }

    public void Event_ResumeGameLogic()
    {
        foreach (Humanoid humanoid in aiArray)
        {
            humanoid.resumeHumanoidMovement();
        }

        player.GetComponent<PlayerController>().revivePlayer();

        bombSpawnManager.gameObject.SetActive(true);

        canSpawnPickUps = true;
    }    

    private GameObject spawned_Single;
    private Cell c_single;
    private void spawnFloorSingle(int key)
    {
        for (int j = (pickUpCountPerHumanoidPerFloor[keyToFloor[key]] - 1); j >= 0; j--)
        {
            enablePickUps(
                key,
                keyToFloor[key],
                allFloorsUnoccupiedCellIndexes[keyToFloor[key]].unoccupiedCellIndexes[j],
                j
                );
        }

        void enablePickUps(int poolParentIndex, int currentFloor, int cellIndex, int unoccupiedCellIndex)
        {
            //Debug.Log("Single : " + poolParentIndex);
            spawned_Single = pickUpsPooler.GetPooledObject(poolParentIndex);

            spawned_Single.SetActive(true);

            c_single = floors[keyToFloor[key]].cells[allFloorsUnoccupiedCellIndexes[keyToFloor[key]].unoccupiedCellIndexes[unoccupiedCellIndex]];

            spawned_Single.GetComponent<PickUp>().dePool(
                currentFloor, 
                cellIndex,
                c_single.position,
                c_single.rotation, 
                unoccupiedCellIndex
                );

            floors[currentFloor].cells[cellIndex].occupied = true;
            allFloorsUnoccupiedCellIndexes[currentFloor].unoccupiedCellIndexes.RemoveAt(unoccupiedCellIndex);
        }
    }
    
    private void Update()
    {
        if (canSpawnPickUps)
        {
            spawnPickUps();
        }
    }

    public void Event_OnPickUpPickedUp(PickUpPickedUp p)
    {
        floors[p.dePooledFloor].cells[p.dePooledIndex].occupied = false;
        allFloorsUnoccupiedCellIndexes[p.dePooledFloor].unoccupiedCellIndexes.Add(p.dePooledIndex);
    }

    private int floorIndex;
    private int randomUnoccupiedCellIndex;

    private Vector3 pos;
    private Quaternion rot;
    private GameObject spawned;
    private void spawnPickUps()
    {
        for (int i = 0; i < allHumanoidRepooledPickUps.Length; i++)
        {
            if (allHumanoidRepooledPickUps[i].repooledPickUps.Count > 0)
            {
                floorIndex = keyToFloor[i];

                randomUnoccupiedCellIndex = UnityEngine.Random.Range(0, allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes.Count);

                pos = floors[floorIndex].cells[allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes[randomUnoccupiedCellIndex]].position;
                rot = floors[floorIndex].cells[allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes[randomUnoccupiedCellIndex]].rotation;
                spawned = allHumanoidRepooledPickUps[i].repooledPickUps[0].gameObject;
                spawned.SetActive(true);
                spawned.GetComponent<PickUp>().dePool(floorIndex, allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes[randomUnoccupiedCellIndex], pos,rot, randomUnoccupiedCellIndex);

                floors[floorIndex].cells[allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes[randomUnoccupiedCellIndex]].occupied = true;
                allFloorsUnoccupiedCellIndexes[floorIndex].unoccupiedCellIndexes.RemoveAt(randomUnoccupiedCellIndex);

                allHumanoidRepooledPickUps[i].repooledPickUps.RemoveAt(0);                
            }
        }
    }   

    public void changeFloorForHumanoid(int key, int newFloor)
    {
        keyToFloor[key] = newFloor;

        Transform pool = pickUpsPooler.transform.GetChild(key);
        for (int i = pool.childCount - 1; i >= 0; i--)
        {
            pool.GetChild(i).gameObject.SetActive(false);
        }

        spawnFloorSingle(key);
        //return;

        //StartCoroutine(wait());
        //System.Collections.IEnumerator wait()
        //{
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    spawnFloorSingle(key, newFloor);
        //}        
    }

    public void humanoidWON(Humanoid winner)
    {
        this.winner = winner;

        if (winner.CompareTag("player"))
        {
            OnPlayerWonRace.Raise();

            
        }
        else if(winner.CompareTag("ai"))
        {
            OnPlayerLostRace.Raise();
        }
        else
        {
            Debug.LogWarning("this should not have happened at all");
        }
    }    

    public void Event_OnPlayerWon()
    {
        Event_StopGameLogic();
        managePodium();
    }

    public void Event_OnPlayerLost()
    {
        Event_StopGameLogic();
        managePodium();
    }

    public void Event_OnPlayerEliminated()
    {
        foreach (Humanoid humanoid in humanoidArray)
        {
            humanoid.pauseHumanoidMovement();
        }

        canSpawnPickUps = false;
    }

    private void managePodium()
    {
        List<Humanoid> sortedHumanoids = new List<Humanoid>();

        sortedHumanoids.Add(winner);

        sortHumanoids();
        void sortHumanoids()
        {
            foreach (Humanoid humanoid in humanoidArray)
            {
                if (humanoid != winner && !humanoid.IsEliminated)
                {
                    sortedHumanoids.Add(humanoid);
                }
            }

            // first bubble sort it by its floor
            for (int i = 0; i < sortedHumanoids.Count; i++)
            {
                for (int j = 0; j < sortedHumanoids.Count - 1; j++)
                {
                    if (sortedHumanoids[j].CurrentFloor < sortedHumanoids[j + 1].CurrentFloor)
                    {
                        Humanoid h = sortedHumanoids[j];
                        sortedHumanoids[j] = sortedHumanoids[j + 1];
                        sortedHumanoids[j + 1] = h;
                    }
                }
            }

            // then sort it by the pick up count
            //for (int i = 0; i < sortedHumanoids.Count; i++)
            //{
            //    for (int j = 0; j < sortedHumanoids.Count - 1; j++)
            //    {
            //        if (sortedHumanoids[j].CurrentFloor == sortedHumanoids[j + 1].CurrentFloor)
            //        {
            //            if (sortedHumanoids[j].CurrentPickUpCount < sortedHumanoids[j + 1].CurrentPickUpCount)
            //            {
            //                Humanoid h = sortedHumanoids[j];
            //                sortedHumanoids[j] = sortedHumanoids[j + 1];
            //                sortedHumanoids[j + 1] = h;
            //            }
            //        }
            //    }
            //}
            // sorting is done
        }

        placeHumanoidsOnPodium();
        void placeHumanoidsOnPodium()
        {
            for (int i = 0; i < sortedHumanoids.Count; i++)
            {
                if (i >= podium.childCount)
                {
                    return;
                }
                else
                {
                    sortedHumanoids[i].transform.position = podium.GetChild(i).position;
                    sortedHumanoids[i].transform.rotation = podium.GetChild(i).rotation;
                    podium.GetChild(i).GetComponentInChildren<Renderer>().material.color = sortedHumanoids[i].ThisHumanoidMaterial.color;
                }
            }
        }

        manageCamera();
        void manageCamera()
        {
            podiumCamera.Priority = 20;
        }
    }
}