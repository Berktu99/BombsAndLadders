using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Damager
{
    public int key = -1;
    public Material mat = null;

    public Damager()
    {
        key = -1;
        mat = null;
    }

    public Damager(int key, Material mat)
    {
        this.key = key;
        this.mat = mat;
    }
}

public enum TargetType
{
    Destination,
    Bombing,
}

public class Target<T> where T : IamTarget
{
    public IamTarget targetedObj = null;

    public Vector3 targetPos = Vector3.zero;

    public TargetType targetType;

    public Bomb bomb = null;
    public LadderPlatform platform;
    public Transform humanoidTransform = null;

    public Target()
    {
        targetedObj = null;
        targetPos = Vector3.zero;

        bomb = null;
        humanoidTransform = null;
        this.platform = null;
    }

    public Target(T t, Vector3 targetPos, TargetType targetType)
    {
        targetedObj = t;
        this.targetPos = targetPos;
        this.targetType = targetType;

        bomb = null;
        humanoidTransform = null;
        this.platform = null;
    }

    public Target(T t, Vector3 targetPos, TargetType targetType, Bomb bomb)
    {
        targetedObj = t;
        this.targetPos = targetPos;
        this.targetType = targetType;

        this.bomb = bomb;
        this.humanoidTransform = null;
        this.platform = null;
    }

    public Target(T t, Vector3 targetPos, TargetType targetType, Transform humanoidTransform)
    {
        targetedObj = t;
        this.targetPos = targetPos;
        this.targetType = targetType;

        this.bomb = null;
        this.humanoidTransform = humanoidTransform;
        this.platform = null;
    }

    public Target(T t, Vector3 targetPos, TargetType targetType, LadderPlatform lp)
    {
        targetedObj = t;
        this.targetPos = targetPos;
        this.targetType = targetType;

        this.platform = lp;
        this.humanoidTransform = null;
        this.bomb = null;
    }
}

public class HumanoidPickUpVariables
{
    public bool canCollectPickUp;
    public int keyValue;
    public Transform pickUpPackTr;
    public Color32 color;
    public Material mat;

    public HumanoidPickUpVariables(Transform pickUpPackTr, Color32 color, int keyValue)
    {
        this.pickUpPackTr = pickUpPackTr;
        this.color = color;
        canCollectPickUp = true;
        this.keyValue = keyValue;
    }

    public HumanoidPickUpVariables(Transform pickUpPackTr, Material mat, int keyValue)
    {
        this.pickUpPackTr = pickUpPackTr;
        this.mat = mat;
        canCollectPickUp = true;
        this.keyValue = keyValue;
    }

    public void updateVariables(bool canCollectPickUp)
    {
        this.canCollectPickUp = canCollectPickUp;
    }
}

public enum AnimationHashes
{
    isIdleBoolHash,
    isIdleCarryingBoolHash,
    isRunEmptyBoolHash,
    isRunCarryingBoolHash,
    isKnockedDownBoolHash,
    isClimbingBoolHash,
    isFallingBoolHash,
    isClimbingTopTreeBoolHash,
    podiumPoseTriggerHash,
}

public class Humanoid : MonoBehaviour, IamTarget
{
    #region STATE VARIABLES
    public enum States
    {
        Waiting,
        IsKnockedDown,
        IsOnTheLadder,
        SearchingForTarget,
        TargetsPickUp,
        TargetsBomb,
        TargetsHumanoid,
        TargetsLadderPlatform,
    }

    private States currentState;
    #endregion

    #region CUSTOMIZATION VARIABLES
    [MyBox.Foldout("CUSTOMIZATION VARIABLES", true)]
    [SerializeField] protected GameObject nameTag;
    public Transform gfxParent;
    public Transform headAccessoriesParent;

    private Camera mainCamera;

    protected Color32 humanoidColor = Color.magenta;
    [Space(4)]
    #endregion

    #region GENERAL VARIABLES
    [MyBox.Foldout("GENERAL VARIABLES", true)]
    [SerializeField] private GameObject nullTransform;
    public Renderer defaultSkinRenderer;
    [SerializeField] protected int currentFloor = 0;

    public int CurrentFloor => currentFloor;
    private int lastFloor;
    [SerializeField] protected LayerMask groundMask;
    protected bool humanoidMovementIsStopped = true;

    protected Transform selfTransform;

    [SerializeField] private Vector3 climbTopOffsetVector;
    [SerializeField] private float climbTopOffsetTime;
    private enum ClimbingTopTree
    {
        ClimbTop = 1,
        ClimbCoyote = -1,
    }
    private ClimbingTopTree whichAnim;

    [SerializeField] private Vector3 climbTopCoyoteOffsetVector;
    [SerializeField] private float climbTopCoyoteOffsetTime;

    protected CharacterController characterController;
    public Animator animator;

    protected bool isClimbingTopTree = false;
    protected bool isMovementPressed = false;
    protected bool isKnockedDown = false;
    protected bool isONtheLadder = false;

    [SerializeField] private LayerMask eliminationMask;
    [SerializeField] protected float registerDmgTime = 5f;
    protected bool isEliminated = false;
    public bool IsEliminated => isEliminated;
    protected float registerDmgTimer = 0f;
    protected Damager lastDamager = new Damager();

    private LevelManager levelManager;

    protected Material thisHumanoidMaterial;
    public Material ThisHumanoidMaterial => thisHumanoidMaterial;
    [Space(4)]
    #endregion

    #region MOVEMENT VARIABLES
    [MyBox.Foldout("MOVEMENT VARIABLES", true)]
    [SerializeField] protected float speed = 6.0f;
    [SerializeField] protected float gravity = 20.0f;
    [SerializeField] protected float rotationFactorPerFrame = 1f;
    [SerializeField] private float knockOutTime = 1.5f;
    [SerializeField] protected float ladderClimbSpeed = 2f;
    private float ladderClimbSpeedProtected;

    protected Vector3 currentMovement = Vector3.zero;
    protected float climbingDirection = 0;
    [Space(5)]
    #endregion

    # region AI RELATED VARIABLES
    [MyBox.Foldout("AI RELATED VARIABLES", true)]

    [SerializeField, Range(0, 100)] private int goalDriven;
    [SerializeField] private int pickUpCountForGoal = 5;
    [SerializeField, Range(0, 100)] private int vindictiveness;
    [Tooltip("Intelligence level 0 means no decision abilities, level 1 means it has some info about the world to decide an outcome level 2 means it has all the necessary info to take the best course of action at all times.")]
    [SerializeField, Range(0, 2)] private int intelligence;

    private NavMeshAgent navMeshAgent;

    private Target<IamTarget> currentTarget;

    private bool destinationTargetAcquired = false;

    private bool bombingTargetAcquired = false;

    [SerializeField] protected float afterThrowBombLockTime = 1f;

    [HideInInspector] public Humanoid[] opponentHumanoids;
    [SerializeField] private int navMeshRefresh = 30;

    private Transform ladderPlatformsParent;
    private List<LadderPlatform[]> allLadderPlatforms;

    private bool canLookForBetterOpportunity = true;
    [SerializeField] private float opportunityRadius = 4f;
    [SerializeField] private LayerMask opportunitiesLayer;

    [Space(5)]
    #endregion

    #region PICK-UP VARIABLES
    [MyBox.Foldout("PICK-UP VARIABLES", true)]
    [SerializeField] public int currentPickUpCount = 0;
    public int CurrentPickUpCount => currentPickUpCount;
    [SerializeField] protected float pickUpOverlapCapsuleRadius = 2f;
    protected Transform pickUpPackTransform;
    [SerializeField] private Vector3 pickUpPackLocalPos;
    [SerializeField] protected LayerMask layerMaskPickUp;
    protected bool canCollectPickUp = true;

    [SerializeField] private float pickUpTime = 0.6f;
    private float pickUpTimer;
    [SerializeField] protected int key; public int Key => key;

    [HideInInspector] public HumanoidPickUpVariables pickUpVariables;
    [Space(4)]
    #endregion

    #region LADDER STACKING VARIABLES
    [MyBox.Foldout("LADDER STACKING VARIABLES", true)]
    [SerializeField] protected bool isFallingFromLadder = false;
    [SerializeField] protected bool insideTrigger_LadderPlatform = false;
    [SerializeField] protected LadderPlatform[] designatedLadderPlatforms;
    [SerializeField] protected ObjectPooler objectPooler;
    [SerializeField] protected float ladderHeight;
    [SerializeField] protected float stackingCorrectionDeltaY = 0.7f;
    [SerializeField] protected float ladderPlatformOverlapCapsuleRadius = 2f;
    [SerializeField] protected LayerMask ladderPlatformMask;
    [Space(4)]
    #endregion

    #region BOMB RELATED VARIABLES
    [MyBox.Foldout("BOMB RELATED VARIABLES", true)]
    [SerializeField] protected bool canTakeDirectBombExplosion = true;
    [SerializeField] protected Bomb carriedBomb = null;
    [SerializeField] protected bool isCarryingBomb = false;
    [SerializeField] protected Trajectory trajectory;
    private Transform bombCarryTransform;
    [SerializeField] private Vector3 bombCarryLocalPos;
    [SerializeField] protected Vector3 bombThrowVector = Vector3.up;
    [SerializeField] protected float bombThrowSpeed = 3f;
    [SerializeField] protected Vector3 bombThrowPos;
    [Space(4)]
    #endregion

    #region ANIMATION VARIABLES
    //[MyBox.Foldout("ANIMATION VARIABLES",true)]   
    private int isIdleBoolHash;
    private int isIdleCarryingBoolHash;
    private int isRunEmptyBoolHash;
    private int isRunCarryingBoolHash;
    private int isKnockedDownBoolHash;
    private int isClimbingBoolHash;
    private int isFallingBoolHash;
    private int isClimbingTopTreeBoolHash;
    private int podiumPoseTriggerHash;
    private int idleTriggerHash;
    #endregion

    #region EVENTS
    [MyBox.Foldout("EVENTS", true)]
    public HumanoidEliminationEvent onHumanoidEliminated_Self;
    public HumanoidEliminationEvent onHumanoidEliminated_ByOther;
    public VoidEvent onPlayerEliminated;

    public VoidEvent OnPlayerWonRace;
    public VoidEvent OnPlayerLostRace;

    [SerializeField] protected VoidEvent onPickUpPickedUp;
    [SerializeField] private VoidEvent onLadderClimbed;

    [SerializeField] private VoidEvent onChangeFloor;
    #endregion

    public virtual void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        selfTransform = GetComponent<Transform>();

        ladderClimbSpeedProtected = ladderClimbSpeed;

        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = speed;
        }

        currentTarget = new Target<IamTarget>();

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        trajectory = new Trajectory();

        pickUpTimer = pickUpTime;

        isIdleBoolHash = Animator.StringToHash("IsIdleBool");
        isIdleCarryingBoolHash = Animator.StringToHash("IsIdleCarryingBool");
        isRunEmptyBoolHash = Animator.StringToHash("IsRunEmptyBool");
        isRunCarryingBoolHash = Animator.StringToHash("IsRunCarryingBool");
        isKnockedDownBoolHash = Animator.StringToHash("IsKnockedDownBool");
        isClimbingBoolHash = Animator.StringToHash("IsClimbingBool");
        isFallingBoolHash = Animator.StringToHash("IsFallingBool");
        isClimbingTopTreeBoolHash = Animator.StringToHash("IsClimbingTopTreeBool");
        podiumPoseTriggerHash = Animator.StringToHash("PodiumPoseTrigger");
        idleTriggerHash = Animator.StringToHash("IdleTrigger");

        whichAnim = ClimbingTopTree.ClimbTop;
    }

    public virtual void Start()
    {
        mainCamera = Camera.main;
        objectPooler = ObjectPooler.getInstance();
        ladderHeight = 1f; //objectPooler.GetPooledObject(PoolableItems.LadderGFX).transform.GetChild(0).lossyScale.y;
    }

    public void initHumanoidVariables(int key, LevelManager levelManager)
    {
        this.key = key;

        thisHumanoidMaterial = defaultSkinRenderer.material;

        pickUpVariables = new HumanoidPickUpVariables(pickUpPackTransform, humanoidColor, this.key);

        this.levelManager = levelManager;

        ladderPlatformsParent = this.levelManager.transform.GetChild(2);
        allLadderPlatforms = new List<LadderPlatform[]>(ladderPlatformsParent.childCount);

        lastFloor = levelManager.FloorCount - 2;

        for (int i = 0; i < ladderPlatformsParent.childCount; i++)
        {
            allLadderPlatforms.Add(ladderPlatformsParent.GetChild(i).GetComponentsInChildren<LadderPlatform>());
        }

        designatedLadderPlatforms = new LadderPlatform[ladderPlatformsParent.childCount];

        opponentHumanoids = GetComponent<FindOpposingHumanoids>().GetOpposingHumanoids(this);
    }

    public void startHumanoidMovement()
    {
        pickUpVariables = new HumanoidPickUpVariables(pickUpPackTransform, humanoidColor, this.key);

        GetComponent<CharacterController>().enabled = true;

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.enabled = true;
        }

        humanoidMovementIsStopped = false;

        GetComponent<Animator>().SetTrigger(Animator.StringToHash("IdleTrigger"));
    }

    public void pauseHumanoidMovement()
    {
        ladderClimbSpeed = 0;

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.speed = 0;
        }

        humanoidMovementIsStopped = true;
    }

    public void stopHumanoidMovement(bool isPodium)
    {
        GetComponent<CharacterController>().enabled = false;

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.enabled = false;
        }

        humanoidMovementIsStopped = true;

        if (isPodium)
        {
            GetComponent<Animator>().SetTrigger(Animator.StringToHash("PodiumPoseTrigger"));
        }
        else
        {
            GetComponent<Animator>().SetTrigger(Animator.StringToHash("IdleTrigger"));
        }
    }

    public void resumeHumanoidMovement()
    {
        ladderClimbSpeed = ladderClimbSpeedProtected;

        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            agent.speed = speed;
        }

        humanoidMovementIsStopped = false;
    }

    void Update()
    {
        handleGravity();
        handleMovement();
        handleRotation();

        handleClimbing();
        handleAnimation();
        handleRegisterDamage();

        handleNameTag();

        if (isCarryingBomb)
        {
            handleBombTrajectory();
        }
    }

    private void FixedUpdate()
    {
        handleCollisionChecks();
    }

    public void assignName(string s)
    {
        nameTag.GetComponentInChildren<TMPro.TextMeshPro>().text = s;
    }

    public void assignName(SaveStateChange saveStateChange)
    {
        if (saveStateChange.saveStateChangeable == SaveState.SaveStateChangeableVariables.playerName)
        {
            nameTag.GetComponentInChildren<TMPro.TextMeshPro>().text = saveStateChange.newStringValue;
        }
    }

    private void handleNameTag()
    {
        nameTag.transform.LookAt(mainCamera.transform);
        nameTag.transform.eulerAngles =
            new Vector3(
            -5f,
            nameTag.transform.rotation.eulerAngles.y,
            0.0f)
            ;
    }

    public void equipNewCharacterSkin(HumanoidSkinType skinType, GameObject skinPrefab)
    {
        Transform[] toDestroy;
        switch (skinType)
        {
            case HumanoidSkinType.Accessory:
                {
                    // destroy previously spawn shit ON BOTH ACCESSORY channel AND whole body spawn (there should be only 1 instance)    
                    toDestroy = gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    for (int i = 0; i < toDestroy.Length; i++)
                    {
                        Destroy(toDestroy[i].gameObject);
                    }


                    for (int i = headAccessoriesParent.childCount - 1; i >= 0; i--)
                    {
                        Destroy(headAccessoriesParent.GetChild(i).gameObject);
                    }

                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(true);
                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").SetAsFirstSibling();

                    Instantiate(skinPrefab, headAccessoriesParent);

                    animator.Rebind();

                    break;
                }
            case HumanoidSkinType.WholeBody:
                {
                    // this time, do NOT destroy default skin, animator work for only the 0th child under gfx transform
                    // make sure there is only one whole body spawn (except of course deafult skin) at any given time                   

                    // REMEMBER
                    // when instantiating it is all good but when destroying it updates the end of frame

                    toDestroy = gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    if (toDestroy.Length > 1)
                    {
                        Debug.LogError("There should not have been more than 1 whole body skin");
                        return;
                    }
                    else if (toDestroy.Length == 1)
                    {
                        Destroy(toDestroy[0].gameObject);
                    }

                    Instantiate(skinPrefab, gfxParent).transform.SetAsFirstSibling();
                    gfxParent.GetChild(0).tag = "wholeBodySkin";

                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(false);

                    animator.Rebind();
                    break;
                }
            default:
                {
                    Debug.Log("this enum was not assigned for");
                    break;
                }
        }

        // Reset 3 info carrier transforms. PickUpPackTransform, BombCarryTransfom, CollisionCastTransform.

        Transform spine = transform.FindChildWithNameBreadthFirst("mixamorig:Spine");
        for (int i = spine.childCount - 1; i >= 1; i--)
        {
            Destroy(spine.transform.GetChild(i).gameObject);
        }
        GameObject pickUpPackTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        pickUpPackTemp.name = "PickUpPackTransformNEW";
        pickUpPackTemp.transform.localPosition = pickUpPackLocalPos;
        pickUpPackTransform = pickUpPackTemp.transform;

        GameObject bombCarryTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        bombCarryTemp.name = "BombCarryTransformNEW";
        bombCarryTemp.transform.localPosition = bombCarryLocalPos;
        bombCarryTransform = bombCarryTemp.transform;
        trajectory.initialize(bombCarryTransform);
        //GameObject sphereCastTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        //sphereCastTemp.name = "SphereCastTransformNEW";
        //sphereCastTemp.transform.localPosition = sphereCastLocalPos;
        //sphereCastPos = sphereCastTemp.transform;
    }

    public void equipNewCharacterSkin(HumanoidEquip newHumanoidEquip)
    {
        Transform[] toDestroy;
        switch (newHumanoidEquip.humanoidSkinType)
        {
            case HumanoidSkinType.Accessory:
                {
                    // destroy previously spawn shit ON BOTH ACCESSORY channel AND whole body spawn (there should be only 1 instance)    
                    toDestroy = gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    for (int i = 0; i < toDestroy.Length; i++)
                    {
                        Destroy(toDestroy[i].gameObject);
                    }


                    for (int i = headAccessoriesParent.childCount - 1; i >= 0; i--)
                    {
                        Destroy(headAccessoriesParent.GetChild(i).gameObject);
                    }

                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(true);
                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").SetAsFirstSibling();

                    Instantiate(newHumanoidEquip.skinPrefab, headAccessoriesParent);

                    animator.Rebind();

                    break;
                }
            case HumanoidSkinType.WholeBody:
                {
                    // this time, do NOT destroy default skin, animator work for only the 0th child under gfx transform
                    // make sure there is only one whole body spawn (except of course deafult skin) at any given time                   

                    // REMEMBER
                    // when instantiating it is all good but when destroying it updates the end of frame

                    toDestroy = gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    if (toDestroy.Length > 1)
                    {
                        Debug.LogError("There should not have been more than 1 whole body skin");
                        return;
                    }
                    else if (toDestroy.Length == 1)
                    {
                        Destroy(toDestroy[0].gameObject);
                    }

                    Instantiate(newHumanoidEquip.skinPrefab, gfxParent).transform.SetAsFirstSibling();
                    gfxParent.GetChild(0).tag = "wholeBodySkin";

                    gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(false);

                    animator.Rebind();
                    break;
                }
            default:
                {
                    Debug.Log("this enum was not assigned for");
                    break;
                }
        }

        // Reset 3 info carrier transforms. PickUpPackTransform, BombCarryTransfom, CollisionCastTransform.

        Transform spine = transform.FindChildWithNameBreadthFirst("mixamorig:Spine");
        for (int i = spine.childCount - 1; i >= 1; i--)
        {
            Destroy(spine.transform.GetChild(i).gameObject);
        }
        GameObject pickUpPackTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        pickUpPackTemp.name = "PickUpPackTransformNEW";
        pickUpPackTemp.transform.localPosition = pickUpPackLocalPos;
        pickUpPackTransform = pickUpPackTemp.transform;

        GameObject bombCarryTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        bombCarryTemp.name = "BombCarryTransformNEW";
        bombCarryTemp.transform.localPosition = bombCarryLocalPos;
        bombCarryTransform = bombCarryTemp.transform;

        //GameObject sphereCastTemp = Instantiate(nullTransform, transform.FindChildWithNameBreadthFirst("mixamorig:Spine").transform);
        //sphereCastTemp.name = "SphereCastTransformNEW";
        //sphereCastTemp.transform.localPosition = sphereCastLocalPos;
        //sphereCastPos = sphereCastTemp.transform;
    }

    public void equipNewColorSkin(Color32 newSkinColor)
    {
        defaultSkinRenderer.material.color = newSkinColor;

        humanoidColor = newSkinColor;

        Renderer rend = GetComponentInChildren<Renderer>(false);
        foreach (Material material in rend.materials)
        {
            if (material.name == "Humanoid.001 (Instance)")
            {
                material.color = newSkinColor;
            }
        }
    }

    public void equipNewColorSkin(HumanoidEquip newHumanoidEquip)
    {
        defaultSkinRenderer.material.color = newHumanoidEquip.colorSkin;

        humanoidColor = newHumanoidEquip.colorSkin;

        Renderer rend = GetComponentInChildren<Renderer>(false);
        foreach (Material material in rend.materials)
        {
            if (material.name == "Humanoid.001 (Instance)")
            {
                material.color = newHumanoidEquip.colorSkin;
            }
        }
    }


    float groundedGarvity = -0.05f;
    private void handleGravity()
    {
        if (!isONtheLadder)
        {
            if (characterController.isGrounded)
            {
                if (isFallingFromLadder)
                {
                    isFallingFromLadder = false;
                    getKnockDown();
                }

                //float groundedGarvity = -0.05f;
                currentMovement = new Vector3(currentMovement.x, groundedGarvity, currentMovement.z);
            }
            else
            {
                currentMovement = new Vector3(currentMovement.x, currentMovement.y + (gravity * Time.unscaledDeltaTime), currentMovement.z);
            }
        }
    }


    List<LadderPlatform> loadedPlatforms;
    int maxLoadedIndex = 0;
    List<LadderPlatform> emptyPlatforms;

    Collider[] pickUpHitColliders = new Collider[1];
    int pickUpHitCollidersLength;
    public virtual void handleMovement()
    {
        isMovementPressed = navMeshAgent.velocity.magnitude > 0.01f ? true : false;
        currentMovement = navMeshAgent.velocity;

        if (humanoidMovementIsStopped)
        {
            isMovementPressed = false;
            currentMovement = Vector3.zero;

            return;
        }

        if (isKnockedDown)
        {
            //Debug.Log("isknoecked down");
            isMovementPressed = false;
            currentMovement = Vector3.zero;

            return;
        }

        if (isONtheLadder)
        {
            // move it on the ladder,
            // turn off nav mesh for now,           

            if (currentPickUpCount > 0)
            {
                climbingDirection = 1;
                isMovementPressed = true;
                currentMovement = Vector3.zero;
            }
            else
            {
                climbingDirection = -1;
                isMovementPressed = true;
                currentMovement = Vector3.zero;
            }

            return;
        }

        if (currentTarget.targetedObj == null)
        {
            //  we need to choose our target here.
            // either you are to choose bombing or a pick up target.            

            if (isCarryingBomb && !bombingTargetAcquired)
            {
                chooseBombingTarget();

                void chooseBombingTarget()
                {
                    loadedPlatforms = new List<LadderPlatform>();
                    foreach (var lp in allLadderPlatforms[currentFloor])
                    {
                        if (lp.isAssignedToHumanoid && lp.key != key)
                        {
                            loadedPlatforms.Add(lp);
                        }
                    }

                    if (loadedPlatforms.Count <= 0)
                    {
                        chooseOpponentHumanoid();
                    }
                    else if (designatedLadderPlatforms[currentFloor] == null && loadedPlatforms.Count == allLadderPlatforms[currentFloor].Length)
                    {
                        // bases are loaded, we dont have a base, we need hurriyng,                        
                        chooseLadderPlatform();
                    }
                    else
                    {
                        // bases are not loaded, lets see the humanoid situation.
                        // now, i need to see if there are humanoids near here
                        // or i can choose randomly or better, the highest stacked humanoid

                        if (Random.Range(0, 100) <= vindictiveness)
                        {
                            // target humanoid, it knocks them , stunning for 2 sec,
                            // seems pretty vindictive to me.
                            // this humanoid bombs the largest stack
                            chooseOpponentHumanoid();
                        }
                        else
                        {
                            chooseLadderPlatform();
                        }

                    }

                    void chooseOpponentHumanoid()
                    {
                        maxLoadedIndex = 0;
                        //Debug.Log("1");
                        for (int i = 1; i < opponentHumanoids.Length; i++)
                        {
                            if (opponentHumanoids[i].currentPickUpCount > opponentHumanoids[maxLoadedIndex].currentPickUpCount)
                            {
                                //Debug.Log("2");
                                maxLoadedIndex = i;
                            }
                        }

                        acquireAItarget(opponentHumanoids[maxLoadedIndex], opponentHumanoids[maxLoadedIndex].transform.position, TargetType.Bombing, false);

                        Debug.Log("humanoid can be made more intelligent opportunity");
                        // you can do many things here,                    
                        // or it also calculates if bomb will explode in time going there
                        // and decides
                    }

                    void chooseLadderPlatform()
                    {
                        Debug.Log("humanoid can be made more intelligent opportunity");
                        // although we can also check for the bomb timer to
                        // drop the bomb just in time so no one can pick up?

                        maxLoadedIndex = 0;
                        for (int i = 1; i < loadedPlatforms.Count; i++)
                        {
                            if (loadedPlatforms[i].transform.GetChild(1).childCount > loadedPlatforms[maxLoadedIndex].transform.GetChild(1).childCount)
                            {
                                maxLoadedIndex = i;
                            }
                        }

                        acquireAItarget(loadedPlatforms[maxLoadedIndex], loadedPlatforms[maxLoadedIndex].transform.position, TargetType.Bombing, false);
                        return;
                    }
                }
            }
            else if (!isCarryingBomb && !destinationTargetAcquired)
            {
                // Ladder platforms check
                if (Random.Range(0, 100) < goalDriven && currentPickUpCount >= pickUpCountForGoal)
                {
                    if (designatedLadderPlatforms[currentFloor] != null)
                    {
                        acquireAItarget(designatedLadderPlatforms[currentFloor], designatedLadderPlatforms[currentFloor].transform.position, TargetType.Destination, false);
                        return;
                    }
                    else
                    {
                        emptyPlatforms = new List<LadderPlatform>();
                        foreach (var lp in allLadderPlatforms[currentFloor])
                        {
                            if (!lp.isAssignedToHumanoid)
                            {
                                emptyPlatforms.Add(lp);
                            }
                        }

                        if (emptyPlatforms.Count <= 0)
                        {
                            Debug.Log("bases are loaded, we need to hurry to find a bomb. which is not implemented. YET");
                            return;
                        }

                        LadderPlatform t = emptyPlatforms[Random.Range(0, emptyPlatforms.Count)];
                        acquireAItarget(t, t.transform.position, TargetType.Destination, false);
                        return;
                    }
                }

                // first cast a LARGE box, more reliable from list wandering, more expensive too i think.
                lookForTarget();

                void lookForTarget()
                {
                    Collider[] pickUpHitColliders = Physics.OverlapBox(this.transform.position, new Vector3(opportunityRadius * 10f, 3f, opportunityRadius * 10f), Quaternion.identity, opportunitiesLayer, QueryTriggerInteraction.Collide);

                    if (pickUpHitColliders.Length > 0)
                    {
                        foreach (var hitCollider in pickUpHitColliders)
                        {
                            if (!isCarryingBomb)
                            {
                                if (hitCollider.TryGetComponent<Bomb>(out Bomb b))
                                {
                                    if (!b.pickUpAble && Random.Range(0, 100) < vindictiveness)
                                    {
                                        acquireAItarget(b, b.transform.position, TargetType.Destination, true);
                                        break;
                                    }
                                }
                            }


                            if (hitCollider.TryGetComponent<PickUp>(out PickUp t))
                            {
                                if (t.compareKey(key))
                                {
                                    acquireAItarget(t, t.transform.position, TargetType.Destination, true);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Even though i casted a large box, if it did not hit anything pick up able
                        // do something ?
                        Debug.Log("do something for if every pick up is collected and stuff.");
                    }
                }
            }
        }
        else
        {
            if (currentTarget.targetType == TargetType.Destination)
            {
                if (canLookForBetterOpportunity)
                {
                    lookForBetterOpportunity();
                    void lookForBetterOpportunity()
                    {
                        //Collider[] pickUpHitColliders = Physics.OverlapSphere(this.transform.position, opportunityRadius, opportunitiesLayer, QueryTriggerInteraction.Collide);

                        pickUpHitCollidersLength = Physics.OverlapSphereNonAlloc(this.transform.position, opportunityRadius, pickUpHitColliders, opportunitiesLayer, QueryTriggerInteraction.Collide);

                        if (pickUpHitCollidersLength > 0)
                        {
                            if (!isCarryingBomb)
                            {
                                if (pickUpHitColliders[0].TryGetComponent<Bomb>(out Bomb b))
                                {
                                    if (!b.pickUpAble && Random.Range(0, 100) < vindictiveness)
                                    {
                                        acquireAItarget(b, b.transform.position, TargetType.Destination, false);
                                        return;
                                    }
                                }
                            }


                            if (pickUpHitColliders[0].TryGetComponent<PickUp>(out PickUp t))
                            {
                                if (t.compareKey(key))
                                {
                                    acquireAItarget(t, t.transform.position, TargetType.Destination, false);
                                    return;
                                }
                            }
                        }

                        #region discard
                        //if (pickUpHitColliders.Length > 0)
                        //{
                        //    foreach (var hitCollider in pickUpHitColliders)
                        //    {
                        //        Debug.Log("humanoid can be made more intelligent opportunity");
                        //        if (!isCarryingBomb)
                        //        {
                        //            if (hitCollider.TryGetComponent<Bomb>(out Bomb b))
                        //            {
                        //                if (!b.pickUpAble && Random.Range(0, 100) < vindictiveness)
                        //                {
                        //                    acquireAItarget(b, b.transform.position, TargetType.Destination, false);

                        //                    break;
                        //                }
                        //            }
                        //        }


                        //        if (hitCollider.TryGetComponent<PickUp>(out PickUp t))
                        //        {
                        //            if (t.compareKey(key))
                        //            {
                        //                acquireAItarget(t, t.transform.position, TargetType.Destination, false);

                        //                break;
                        //            }
                        //        }
                        //    }
                        //} 
                        #endregion
                    }
                }

                if (currentTarget.targetedObj is Bomb)
                {
                    if (!isCarryingBomb)
                    {
                        // trying to reach to the bomb,
                        // if it is picked up by someone
                        // reset target
                        Debug.Log("humanoid can be made more intelligent opportunity");

                        if (currentTarget.bomb.currentlyPickedUp)
                        {
                            resetAItarget();
                        }
                    }
                }
                else if (currentTarget.targetedObj is LadderPlatform)
                {
                    if (!isONtheLadder)
                    {
                        // trying to reach to the platform,
                        // if it is picked up by someone
                        // reset target, 

                        Debug.Log("humanoid can be made more intelligent opportunity");
                        // if a bomb is placed on our designated platform,
                        // do not try to approach maybe?
                        // or maybe fear of bomb > goal driveness most of the time?

                        if (currentTarget.platform.isAssignedToHumanoid && currentTarget.platform.key != key)
                        {
                            resetAItarget();
                        }
                    }
                }

                if (Helpers.performantDistance(this.transform.position, currentTarget.targetPos) <= pickUpOverlapCapsuleRadius * 0.1f)
                {
                    resetAItarget();
                }

            }
            else if (currentTarget.targetType == TargetType.Bombing)
            {
                if (currentTarget.targetedObj is Humanoid)
                {
                    if (Time.frameCount % navMeshRefresh == 0)
                    {
                        navMeshAgent.SetDestination(currentTarget.humanoidTransform.position);
                    }

                    if (Helpers.performantDistance(this.transform.position, currentTarget.humanoidTransform.position) >= bombThrowSpeed * 0.5f &&
                        Helpers.performantDistance(this.transform.position, currentTarget.humanoidTransform.position) <= bombThrowSpeed * 3)
                    {
                        throwBomb();
                        resetAItarget();
                    }
                }
                else if (currentTarget.targetedObj is LadderPlatform)
                {
                    if (Helpers.performantDistance(this.transform.position, currentTarget.targetPos) <= bombThrowSpeed)
                    {
                        throwBomb();
                        resetAItarget();
                    }
                }
                else
                {
                    Debug.LogError("type is wrong dude come on ...");
                }
            }
        }
    }

    private void resetAItarget()
    {
        currentTarget = new Target<IamTarget>();

        destinationTargetAcquired = false;
        canLookForBetterOpportunity = false;
        bombingTargetAcquired = false;

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = true;
        }
    }

    private void acquireAItarget(IamTarget target, Vector3 pos, TargetType targetType, bool canLookForBetterOpportunity)
    {
        currentTarget = new Target<IamTarget>(target, pos, targetType);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTarget.targetPos);

        this.canLookForBetterOpportunity = canLookForBetterOpportunity;

        if (targetType == TargetType.Bombing)
        {
            destinationTargetAcquired = false;
            bombingTargetAcquired = true;
        }
        else if (targetType == TargetType.Destination)
        {
            destinationTargetAcquired = true;
            bombingTargetAcquired = false;
        }
    }

    private void acquireAItarget(Bomb b, Vector3 pos, TargetType targetType, bool canLookForBetterOpportunity)
    {
        currentTarget = new Target<IamTarget>(b, pos, targetType, b);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTarget.targetPos);

        this.canLookForBetterOpportunity = canLookForBetterOpportunity;

        if (targetType == TargetType.Bombing)
        {
            destinationTargetAcquired = false;
            bombingTargetAcquired = true;
        }
        else if (targetType == TargetType.Destination)
        {
            destinationTargetAcquired = true;
            bombingTargetAcquired = false;
        }
    }

    private void acquireAItarget(Humanoid h, Vector3 pos, TargetType targetType, bool canLookForBetterOpportunity)
    {
        currentTarget = new Target<IamTarget>(h, pos, targetType, h.transform);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTarget.targetPos);

        this.canLookForBetterOpportunity = canLookForBetterOpportunity;

        if (targetType == TargetType.Bombing)
        {
            destinationTargetAcquired = false;
            bombingTargetAcquired = true;
        }
        else if (targetType == TargetType.Destination)
        {
            destinationTargetAcquired = true;
            bombingTargetAcquired = false;
        }
    }

    private void acquireAItarget(LadderPlatform lp, Vector3 pos, TargetType targetType, bool canLookForBetterOpportunity)
    {
        currentTarget = new Target<IamTarget>(lp, pos, targetType, lp);

        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentTarget.targetPos);

        this.canLookForBetterOpportunity = canLookForBetterOpportunity;

        if (targetType == TargetType.Bombing)
        {
            destinationTargetAcquired = false;
            bombingTargetAcquired = true;
        }
        else if (targetType == TargetType.Destination)
        {
            destinationTargetAcquired = true;
            bombingTargetAcquired = false;
        }
    }

    protected Collider[] ladderCollider = new Collider[1];
    protected int ladderColliderLength;
    protected LadderPlatform lp;

    protected Collider[] pickUpHitColliders_handleColCheck = new Collider[1];
    protected int pickUpHitColliders_handleColCheck_Length;

    protected Collider[] groundHit_handleColCheck = new Collider[1];
    protected int groundHit_handleColCheck_Length;
    public virtual void handleCollisionChecks()
    {
        handleLadderPlatformCheck();
        void handleLadderPlatformCheck()
        {
            ladderColliderLength = Physics.OverlapSphereNonAlloc(selfTransform.localPosition, ladderPlatformOverlapCapsuleRadius, ladderCollider, ladderPlatformMask, QueryTriggerInteraction.Collide);

            if (ladderColliderLength > 1)
            {
                Debug.LogError("ladder platforms are too close to each other");
                return;
            }
            else if (ladderColliderLength <= 0)
            {
                if (insideTrigger_LadderPlatform)
                {
                    insideTrigger_LadderPlatform = false;
                }
                return;
            }

            lp = ladderCollider[0].GetComponent<LadderPlatform>();

            if (!isONtheLadder && !isFallingFromLadder)
            {
                if (!isCarryingBomb && !insideTrigger_LadderPlatform)
                {
                    if (pickUpPackTransform.childCount <= 0)
                    {
                        return;
                    }

                    // check ladder begin situation.
                    if (lp.compareKey(-1))
                    {
                        if (designatedLadderPlatforms[currentFloor] == null)
                        {
                            lp.assignHumanoid(pickUpVariables);
                            beginClimb();
                        }
                    }
                    else
                    {
                        if (lp.compareKey(key))
                        {
                            beginClimb();
                        }
                    }

                    void beginClimb()
                    {
                        if (designatedLadderPlatforms[currentFloor] == null)
                        {
                            assignPlatformToHumanoid(lp);
                        }

                        canTakeDirectBombExplosion = false;

                        isONtheLadder = true;
                        climbingDirection = 1;

                        if (navMeshAgent != null)
                        {
                            resetAItarget();

                            navMeshAgent.isStopped = true;
                            climbingDirection = 0;
                            navMeshAgent.enabled = false;
                        }

                        currentMovement = Vector3.zero;

                        characterController.Move(lp.transform.GetChild(2).position + (1.2f * Vector3.up) - this.transform.position);

                        selfTransform.eulerAngles = lp.transform.GetChild(2).eulerAngles;
                    }
                }
            }
            else
            {
                // this is beautiful right here,
                // checks if grounded, by enter trigger.

                transform.position = lp.transform.position;

                //currentMovement = Vector3.zero;

                isONtheLadder = false;
                insideTrigger_LadderPlatform = true;
                climbingDirection = 0;

                if (navMeshAgent != null)
                {
                    resetAItarget();
                }
            }

        }

        handlePickUp();
        void handlePickUp()
        {
            pickUpVariables.updateVariables(canCollectPickUp);

            if (!canCollectPickUp)
            {
                currentPickUpCount = pickUpPackTransform.childCount;
                return;
            }

            pickUpHitColliders_handleColCheck_Length = Physics.OverlapSphereNonAlloc(selfTransform.localPosition, pickUpOverlapCapsuleRadius, pickUpHitColliders_handleColCheck, layerMaskPickUp, QueryTriggerInteraction.Collide);

            if (pickUpHitColliders_handleColCheck_Length > 0)
            {
                if (pickUpHitColliders_handleColCheck[0].TryGetComponent<PickUp>(out PickUp t))
                {
                    if (t.compareKey(key))
                    {
                        t.makeThisPickUpPickedUp(pickUpPackTransform, thisHumanoidMaterial, key);
                        currentPickUpCount++;

                        onPickUpPickedUp.Raise();
                    }
                }
            }

            currentPickUpCount = pickUpPackTransform.childCount;

            #region discard
            //foreach (var hitCollider in pickUpHitColliders_handleColCheck)
            //{
            //    if (hitCollider.TryGetComponent<PickUp>(out PickUp t))
            //    {
            //        if (t.compareKey(key))
            //        {
            //            t.makeThisPickUpPickedUp(pickUpPackTransform, thisHumanoidMaterial, key);
            //            currentPickUpCount++;

            //            break;
            //        }
            //    }
            //}

            //currentPickUpCount = pickUpPackTransform.childCount; 
            #endregion
        }

        handleGroundCheck();
        void handleGroundCheck()
        {
            //Collider[] groundHit_handleColCheck = Physics.OverlapBox(sphereCastPos.position, new Vector3(0.01f, 1f, 0.01f), Quaternion.identity, groundMask, QueryTriggerInteraction.Collide);//Physics.OverlapSphere(sphereCastPos.position, 0.5f, groundMask, QueryTriggerInteraction.Collide);

            groundHit_handleColCheck_Length = Physics.OverlapSphereNonAlloc(selfTransform.localPosition, 0.1f, groundHit_handleColCheck, groundMask, QueryTriggerInteraction.Collide);//Physics.OverlapSphere(sphereCastPos.position, 0.5f, groundMask, QueryTriggerInteraction.Collide);

            if (groundHit_handleColCheck_Length > 0)
            {
                switch (groundHit_handleColCheck[0].gameObject.tag)
                {
                    case "zero":
                        {
                            if (currentFloor != 0)
                            {
                                changeFloor(0);
                            }

                            break;
                        }

                    case "one":
                        {
                            if (currentFloor != 1)
                            {
                                changeFloor(1);
                            }

                            break;
                        }

                    case "two":
                        {
                            if (currentFloor != 2)
                            {
                                changeFloor(2);
                            }

                            break;
                        }

                    case "three":
                        {
                            if (currentFloor != 3)
                            {
                                changeFloor(3);
                            }

                            break;
                        }

                    case "four":
                        {
                            if (currentFloor != 4)
                            {
                                changeFloor(4);
                            }

                            break;
                        }

                    case "five":
                        {
                            if (currentFloor != 5)
                            {
                                changeFloor(5);
                            }

                            break;
                        }
                    default:
                        {
                            if (!isEliminated)
                            {
                                eliminateThisHumanoid();
                            }

                            break;
                        }
                }
            }

            #region discard
            //if (groundHit_handleColCheck.Length > 0)
            //{
            //    switch (groundHit_handleColCheck[0].gameObject.tag)
            //    {
            //        case "zero":
            //            {
            //                if (currentFloor != 0)
            //                {
            //                    changeFloor(0);
            //                }

            //                break;
            //            }

            //        case "one":
            //            {
            //                if (currentFloor != 1)
            //                {
            //                    changeFloor(1);
            //                }

            //                break;
            //            }

            //        case "two":
            //            {
            //                if (currentFloor != 2)
            //                {
            //                    changeFloor(2);
            //                }

            //                break;
            //            }

            //        case "three":
            //            {
            //                if (currentFloor != 3)
            //                {
            //                    changeFloor(3);
            //                }

            //                break;
            //            }

            //        case "four":
            //            {
            //                if (currentFloor != 4)
            //                {
            //                    changeFloor(4);
            //                }

            //                break;
            //            }

            //        case "five":
            //            {
            //                if (currentFloor != 5)
            //                {
            //                    changeFloor(5);
            //                }

            //                break;
            //            }
            //        default:
            //            {
            //                if (!isEliminated)
            //                {
            //                    eliminateThisHumanoid();
            //                }                            

            //                break;
            //            }
            //    }                
            //} 
            #endregion
        }
    }

    bool isIdleBool;
    bool isIdleCarryingBool;
    bool isRunEmptyBool;
    bool isRunCarryingBool;
    bool isKnockedDownBool;
    bool isClimbingBool;
    bool isFallingBool;
    bool isClimbingTopTreeBool;
    private void handleAnimation()
    {
        isIdleBool = animator.GetBool(isIdleBoolHash);
        isIdleCarryingBool = animator.GetBool(isIdleCarryingBoolHash);
        isRunEmptyBool = animator.GetBool(isRunEmptyBoolHash);
        isRunCarryingBool = animator.GetBool(isRunCarryingBoolHash);
        isKnockedDownBool = animator.GetBool(isKnockedDownBoolHash);
        isClimbingBool = animator.GetBool(isClimbingBoolHash);
        isFallingBool = animator.GetBool(isFallingBoolHash);
        isClimbingTopTreeBool = animator.GetBool(isClimbingTopTreeBoolHash);

        if (isClimbingTopTree)
        {
            animator.applyRootMotion = true;
        }
        else
        {
            animator.applyRootMotion = false;
        }

        if (isONtheLadder)
        {
            if (isClimbingTopTree)
            {
                if (!isClimbingTopTreeBool)
                {
                    animator.SetBool(isIdleCarryingBoolHash, false);
                    animator.SetBool(isIdleBoolHash, false);
                    animator.SetBool(isRunEmptyBoolHash, false);
                    animator.SetBool(isRunCarryingBoolHash, false);
                    animator.SetBool(isKnockedDownBoolHash, false);
                    animator.SetBool(isClimbingBoolHash, false);
                    animator.SetBool(isFallingBoolHash, false);

                    animator.SetBool(isClimbingTopTreeBoolHash, true);
                    animator.SetFloat("ClimbingTop", (int)whichAnim);

                    animator.applyRootMotion = true;
                    characterController.enabled = false;

                    StartCoroutine(wait());
                    IEnumerator wait()
                    {
                        if (whichAnim == ClimbingTopTree.ClimbTop)
                        {
                            yield return Yielders.CachedWaitForSeconds(climbTopOffsetTime);
                            //yield return new WaitForSeconds(climbTopOffsetTime);
                            isClimbingTopTree = false;
                            isONtheLadder = false;

                            characterController.enabled = true;

                            animator.applyRootMotion = false;


                            resetAItarget();

                        }
                        else
                        {
                            yield return Yielders.CachedWaitForSeconds(climbTopCoyoteOffsetTime);
                            //yield return new WaitForSeconds(climbTopCoyoteOffsetTime);
                            isClimbingTopTree = false;
                            isONtheLadder = false;

                            characterController.enabled = true;

                            animator.applyRootMotion = false;
                            resetAItarget();
                        }
                    }
                }
            }
            else
            {
                if (!isClimbingBool)
                {
                    animator.SetBool(isIdleCarryingBoolHash, false);
                    animator.SetBool(isIdleBoolHash, false);
                    animator.SetBool(isRunEmptyBoolHash, false);
                    animator.SetBool(isRunCarryingBoolHash, false);
                    animator.SetBool(isKnockedDownBoolHash, false);
                    animator.SetBool(isClimbingBoolHash, true);
                    animator.SetBool(isFallingBoolHash, false);

                    animator.SetBool(isClimbingTopTreeBoolHash, false);

                    //animator.SetBool(isClimbingTopBoolHash, false);
                    //animator.SetBool(isClimbingTopCoyoteBoolHash, false);

                }
                else
                {
                    animator.SetFloat("ClimbingDirection", climbingDirection);
                }
            }
        }
        else
        {
            if (isFallingFromLadder)
            {
                if (!isFallingBool)
                {
                    animator.SetBool(isIdleCarryingBoolHash, false);
                    animator.SetBool(isIdleBoolHash, false);
                    animator.SetBool(isRunEmptyBoolHash, false);
                    animator.SetBool(isRunCarryingBoolHash, false);
                    animator.SetBool(isKnockedDownBoolHash, false);
                    animator.SetBool(isClimbingBoolHash, false);
                    animator.SetBool(isFallingBoolHash, true);

                    animator.SetBool(isClimbingTopTreeBoolHash, false);

                    //animator.SetBool(isClimbingTopBoolHash, false);
                    //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                }
            }
            else
            {
                if (isKnockedDown)
                {
                    if (!isKnockedDownBool)
                    {
                        animator.SetBool(isIdleCarryingBoolHash, false);
                        animator.SetBool(isIdleBoolHash, false);
                        animator.SetBool(isRunEmptyBoolHash, false);
                        animator.SetBool(isRunCarryingBoolHash, false);
                        animator.SetBool(isKnockedDownBoolHash, true);
                        animator.SetBool(isClimbingBoolHash, false);
                        animator.SetBool(isFallingBoolHash, false);

                        animator.SetBool(isClimbingTopTreeBoolHash, false);

                        //animator.SetBool(isClimbingTopBoolHash, false);
                        //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                    }
                }
                else
                {
                    if (isMovementPressed)
                    {
                        if (isCarryingBomb)
                        {
                            if (!isRunCarryingBool)
                            {

                                animator.SetBool(isIdleCarryingBoolHash, false);
                                animator.SetBool(isIdleBoolHash, false);
                                animator.SetBool(isRunEmptyBoolHash, false);
                                animator.SetBool(isRunCarryingBoolHash, true);
                                animator.SetBool(isKnockedDownBoolHash, false);
                                animator.SetBool(isClimbingBoolHash, false);
                                animator.SetBool(isFallingBoolHash, false);

                                animator.SetBool(isClimbingTopTreeBoolHash, false);

                                //animator.SetBool(isClimbingTopBoolHash, false);
                                //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                            }
                        }
                        else
                        {
                            if (!isRunEmptyBool)
                            {
                                animator.SetBool(isIdleCarryingBoolHash, false);
                                animator.SetBool(isIdleBoolHash, false);
                                animator.SetBool(isRunEmptyBoolHash, true);
                                animator.SetBool(isRunCarryingBoolHash, false);
                                animator.SetBool(isKnockedDownBoolHash, false);
                                animator.SetBool(isClimbingBoolHash, false);
                                animator.SetBool(isFallingBoolHash, false);

                                animator.SetBool(isClimbingTopTreeBoolHash, false);

                                //animator.SetBool(isClimbingTopBoolHash, false);
                                //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                            }
                        }
                    }
                    else
                    {
                        if (isCarryingBomb)
                        {
                            if (!isIdleCarryingBool)
                            {
                                animator.SetBool(isIdleCarryingBoolHash, true);
                                animator.SetBool(isIdleBoolHash, false);
                                animator.SetBool(isRunEmptyBoolHash, false);
                                animator.SetBool(isRunCarryingBoolHash, false);
                                animator.SetBool(isKnockedDownBoolHash, false);
                                animator.SetBool(isClimbingBoolHash, false);
                                animator.SetBool(isFallingBoolHash, false);

                                animator.SetBool(isClimbingTopTreeBoolHash, false);

                                //animator.SetBool(isClimbingTopBoolHash, false);
                                //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                            }
                        }
                        else
                        {
                            if (!isIdleBool)
                            {
                                animator.SetBool(isIdleCarryingBoolHash, false);
                                animator.SetBool(isIdleBoolHash, true);
                                animator.SetBool(isRunEmptyBoolHash, false);
                                animator.SetBool(isRunCarryingBoolHash, false);
                                animator.SetBool(isKnockedDownBoolHash, false);
                                animator.SetBool(isClimbingBoolHash, false);
                                animator.SetBool(isFallingBoolHash, false);

                                animator.SetBool(isClimbingTopTreeBoolHash, false);

                                //animator.SetBool(isClimbingTopBoolHash, false);
                                //animator.SetBool(isClimbingTopCoyoteBoolHash, false);
                            }
                        }
                    }
                }
            }
        }
    }

    public virtual void handleRotation()
    {
        // navmesh does rotation mate.
    }

    protected void changeFloor(int newFloor)
    {
        levelManager.changeFloorForHumanoid(key, newFloor);

        currentFloor = newFloor;
        onChangeFloor.Raise();
        resetAItarget();
    }

    private void handleRegisterDamage()
    {
        if (lastDamager.key != -1 && registerDmgTimer <= 0f)
        {
            registerDmgTimer = -1f;
            lastDamager = new Damager();
            return;
        }

        registerDmgTimer -= Time.unscaledDeltaTime;
    }

    public void eliminateThisHumanoid()
    {

        if (lastDamager.key == -1)
        {
            managePickUpsOffThePlayer();
            void managePickUpsOffThePlayer()
            {
                onHumanoidEliminated_Self.Raise(new HumanoidElimination(key));
                //Transform pool = pickUpsPooler.transform.GetChild(key);
                //for (int i = pool.childCount - 1; i >= 0; i--)
                //{
                //    pool.GetChild(i).GetComponent<PickUp>().greyOutLadderPickUp();
                //}
            }

            managePickUpsOnThePlayer();
            void managePickUpsOnThePlayer()
            {
                for (int i = pickUpPackTransform.childCount - 1; i >= 0; i--)
                {
                    pickUpPackTransform.GetChild(i).GetComponent<PickUp>().rePoolEliminated();
                }
            }
        }
        else
        {
            managePickUpsOffThePlayer();
            void managePickUpsOffThePlayer()
            {
                onHumanoidEliminated_ByOther.Raise(new HumanoidElimination(key, lastDamager.key, lastDamager.mat));
                //Transform pool = pickUpsPooler.transform.GetChild(key);
                //for (int i = pool.childCount - 1; i >= 0; i--)
                //{
                //    pool.GetChild(i).GetComponent<PickUp>().greyOutLadderPickUp(lastDamager.key, lastDamager.mat);
                //}
            }

            managePickUpsOnThePlayer();
            void managePickUpsOnThePlayer()
            {
                for (int i = pickUpPackTransform.childCount - 1; i >= 0; i--)
                {
                    pickUpPackTransform.GetChild(i).GetComponent<PickUp>().rePoolEliminated(lastDamager.key, lastDamager.mat);
                }
            }
        }


        GetComponent<CharacterController>().enabled = false;
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        if (this.gameObject.CompareTag("player"))
        {
            onPlayerEliminated.Raise();
        }

        Debug.Log("also should remove from the list of ai targets pls. or you can be lazy and check if iseliminated true before deciding to target this ghost");

        isEliminated = true;
    }

    public virtual void tryGetCaughtInBombExplosion(Humanoid humanoidBombedThis)
    {
        if (canTakeDirectBombExplosion)
        {
            getKnockDown();

            lastDamager = new Damager(humanoidBombedThis.Key, humanoidBombedThis.ThisHumanoidMaterial);
            registerDmgTimer = registerDmgTime;

            canTakeDirectBombExplosion = false;

            isCarryingBomb = false;

            return;
        }
    }

    protected void getKnockDown()
    {
        canCollectPickUp = false;
        isKnockedDown = true;

        resetAItarget();

        loseBackPackStack();

        StartCoroutine(knockDownTimer());
        IEnumerator knockDownTimer()
        {
            yield return Yielders.CachedWaitForSeconds(knockOutTime);

            canCollectPickUp = true;
            isKnockedDown = false;
            canTakeDirectBombExplosion = true;
        }
    }

    protected void loseBackPackStack()
    {
        if (currentPickUpCount > 0)
        {
            for (int i = pickUpPackTransform.childCount - 1; i >= 0; i--)
            {
                pickUpPackTransform.GetChild(i).GetComponent<PickUp>().simulateFalling();
            }

            currentPickUpCount = pickUpPackTransform.childCount;
        }
        else
        {
            Debug.Log("Can not lose stack for there isn't any.");
        }
    }

    public virtual bool makeHumanPickUpBomb(Bomb bomb)
    {
        if (!isCarryingBomb)
        {
            isCarryingBomb = true;
            this.carriedBomb = bomb;
            bomb.gameObject.transform.parent = bombCarryTransform;
            bomb.gameObject.transform.localPosition = Vector3.zero;
            bomb.GetComponent<Rigidbody>().isKinematic = true;

            resetAItarget();

            if (currentTarget.bomb != null)
            {
                currentTarget.bomb = null;
                destinationTargetAcquired = false;
                canLookForBetterOpportunity = false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void throwBomb()
    {
        Vector3 vel = new Vector3(transform.forward.x, bombThrowVector.y, transform.forward.z);
        vel.Normalize();
        carriedBomb.getThrown(vel * bombThrowSpeed, afterThrowBombLockTime);
        carriedBomb = null;

        isCarryingBomb = false;
    }

    Vector3 vel;
    public virtual void handleBombTrajectory()
    {
        vel = new Vector3(transform.forward.x, bombThrowVector.y, transform.forward.z);
        vel.Normalize();
        bombThrowPos = trajectory.getHitPos(vel * bombThrowSpeed);
    }

    protected void assignPlatformToHumanoid(LadderPlatform lp)
    {
        designatedLadderPlatforms[currentFloor] = lp;
    }

    float humanNextYpos;
    float nextYPos;
    GameObject ladderFromPool;
    private void handleClimbing()
    {
        currentPickUpCount = pickUpPackTransform.childCount;
        if (isONtheLadder && !isClimbingTopTree && !humanoidMovementIsStopped)
        {
            if (climbingDirection == 1 && currentPickUpCount <= 0)
            {
                if (currentPickUpCount < 0)
                {
                    Debug.Log("Can not have negative number of pick ups on humanoid name: " + selfTransform.name);
                    return;
                }

                humanNextYpos = selfTransform.position.y + (ladderClimbSpeed * Time.unscaledDeltaTime);
                nextYPos = (designatedLadderPlatforms[currentFloor].transform.GetChild(0).childCount * ladderHeight) + designatedLadderPlatforms[currentFloor].transform.position.y;

                if (humanNextYpos + stackingCorrectionDeltaY > nextYPos)
                {
                    climbingDirection = 0;
                }
                else
                {
                    characterController.Move(new Vector3(0.0f, climbingDirection * ladderClimbSpeed, 0.0f) * Time.unscaledDeltaTime);
                }
            }
            else if (climbingDirection == 1 && currentPickUpCount > 0)
            {
                nextYPos = (designatedLadderPlatforms[currentFloor].transform.GetChild(0).childCount * ladderHeight) + designatedLadderPlatforms[currentFloor].transform.position.y;

                if (selfTransform.position.y + stackingCorrectionDeltaY >= nextYPos)
                {
                    nextYPos -= designatedLadderPlatforms[currentFloor].transform.position.y;

                    ladderFromPool = objectPooler.GetPooledObject(PoolableItems.LadderGFX);

                    ladderFromPool.SetActive(true);
                    ladderFromPool.GetComponent<LadderGFX>().matchColors(humanoidColor);
                    ladderFromPool.transform.parent = designatedLadderPlatforms[currentFloor].transform.GetChild(0);
                    ladderFromPool.transform.localPosition = new Vector3(0.0f, nextYPos, 0.0f);
                    ladderFromPool.transform.localRotation = Quaternion.identity;

                    onLadderClimbed.Raise();

                    pickUpPackTransform.GetChild(pickUpPackTransform.childCount - 1).GetComponent<PickUp>().rePool();

                    if (currentFloor == lastFloor)
                    {
                        if (designatedLadderPlatforms[currentFloor].transform.GetChild(0).childCount == levelManager.ladderCountBetweenFloors[currentFloor] - 1)
                        {
                            // Humanoid WON!!                            
                            levelManager.humanoidWON(this);
                            return;
                        }
                    }
                }

                characterController.Move(new Vector3(0.0f, climbingDirection * ladderClimbSpeed, 0.0f) * Time.unscaledDeltaTime);
            }
            else
            {
                characterController.Move(new Vector3(0.0f, climbingDirection * ladderClimbSpeed, 0.0f) * Time.unscaledDeltaTime);
            }



            if (selfTransform.position.y + (stackingCorrectionDeltaY * 2f) >= (levelManager.ladderCountBetweenFloors[currentFloor] * ladderHeight) + designatedLadderPlatforms[currentFloor].transform.position.y)
            {
                isClimbingTopTree = true;
                whichAnim = ClimbingTopTree.ClimbTop;
                //isClimbingTop = true;
                isONtheLadder = true;
            }
        }
    }

    public void fallDownLadder(float height)
    {
        //Debug.Log("fall down the ladder");
        isONtheLadder = false;
        isFallingFromLadder = true;

        currentMovement = transform.forward * -1;
    }
}