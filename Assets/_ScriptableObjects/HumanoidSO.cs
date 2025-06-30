using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HumanoidSO : ScriptableObject
{
    //#region STATE VARIABLES
    //public enum States
    //{
    //    Waiting,
    //    IsKnockedDown,
    //    IsOnTheLadder,
    //    SearchingForTarget,
    //    TargetsPickUp,
    //    TargetsBomb,
    //    TargetsHumanoid,
    //    TargetsLadderPlatform,
    //}

    //private States currentState;
    //#endregion

    //#region CUSTOMIZATION VARIABLES
    //[Header("CUSTOMIZATION VARIABLES")]
    //[SerializeField] private GameObject nameTag;
    //public Transform gfxParent;
    //public Transform headAccessoriesParent;
    //[Space(4)]
    //#endregion

    //#region GENERAL VARIABLES
    //[Header("GENERAL VARIABLES")]
    //[SerializeField] private GameObject nullTransform;
    //[SerializeField] private Vector3 sphereCastLocalPos;
    //private Transform sphereCastPos = null;
    //public Renderer defaultSkinRenderer;
    //[SerializeField] protected int currentFloor = 0;
    //public int CurrentFloor { get { return currentFloor; } }
    //private int lastFloor;
    //[SerializeField] private LayerMask groundMask;
    //protected bool humanoidMovementIsStopped = true;

    //[SerializeField] private Vector3 climbTopOffsetVector;
    //[SerializeField] private float climbTopOffsetTime;
    //private enum ClimbingTopTree
    //{
    //    ClimbTop = 1,
    //    ClimbCoyote = -1,
    //}
    //private ClimbingTopTree whichAnim;

    //[SerializeField] private Vector3 climbTopCoyoteOffsetVector;
    //[SerializeField] private float climbTopCoyoteOffsetTime;

    //protected CharacterController characterController;
    //public Animator animator;

    //protected bool isClimbingTopTree = false;
    //protected bool isMovementPressed = false;
    //protected bool isKnockedDown = false;
    //protected bool isONtheLadder = false;

    //[SerializeField] private LayerMask eliminationMask;
    //[SerializeField] protected float registerDmgTime = 5f;
    //protected bool isEliminated = false;
    //public bool IsEliminated { get { return isEliminated; } }
    //protected float registerDmgTimer = 0f;
    //protected Damager lastDamager = new Damager();

    //private GameManager gameManager;
    //private LevelManager levelManager;
    //private PickUpsPooler pickUpsPooler;

    //private Material thisHumanoidMaterial;
    //public Material ThisHumanoidMaterial { get { return thisHumanoidMaterial; } }
    //[Space(4)]
    //#endregion

    //#region MOVEMENT VARIABLES
    //[Header("MOVEMENT VARIABLES")]
    //[SerializeField] protected float speed = 6.0f;
    //[SerializeField] protected float gravity = 20.0f;
    //[SerializeField] protected float rotationFactorPerFrame = 1f;
    //[SerializeField] private float knockOutTime = 1.5f;
    //[SerializeField] protected float ladderClimbSpeed = 2f;
    //private float ladderClimbSpeedProtected;
    //[SerializeField] protected float ladderClimbInitialYmove = 1f;

    //protected Vector3 currentMovement = Vector3.zero;
    //protected float climbingDirection = 0;
    //[Space(5)]
    //#endregion

    //# region AI RELATED VARIABLES
    //[Header("AI RELATED VARIABLES")]

    //[SerializeField, Range(0, 100)] private int goalDriven;
    //[SerializeField] private int pickUpCountForGoal = 5;
    //[SerializeField, Range(0, 100)] private int vindictiveness;
    //[Tooltip("Intelligence level 0 means no decision abilities, level 1 means it has some info about the world to decide an outcome level 2 means it has all the necessary info to take the best course of action at all times.")]
    //[SerializeField, Range(0, 2)] private int intelligence;

    //private UnityEngine.AI.NavMeshAgent navMeshAgent;

    //private Target<IamTarget> currentTarget;

    //private bool destinationTargetAcquired = false;

    //private bool bombingTargetAcquired = false;

    //[SerializeField] protected float afterThrowBombLockTime = 1f;

    //private Humanoid[] opponentHumanoids;
    //[SerializeField] private int navMeshRefresh = 30;

    //private Transform ladderPlatformsParent;
    //private List<LadderPlatform[]> allLadderPlatforms;

    //private bool canLookForBetterOpportunity = true;
    //[SerializeField] private float opportunityRadius = 4f;
    //[SerializeField] private LayerMask opportunitiesLayer;

    //[Space(5)]
    //#endregion

    //#region PICK-UP VARIABLES
    //[Header("PICK-UP VARIABLES")]
    //[SerializeField] public int currentPickUpCount = 0;
    //public int CurrentPickUpCount { get { return currentPickUpCount; } }
    //[SerializeField] private float pickUpOverlapCapsuleRadius = 2f;
    //private Transform pickUpPackTransform;
    //[SerializeField] private Vector3 pickUpPackLocalPos;
    //[SerializeField] private LayerMask layerMaskPickUp;
    //private bool canCollectPickUp = true;

    //[SerializeField] private int key; public int Key { get { return key; } }

    //[HideInInspector] public HumanoidPickUpVariables pickUpVariables;
    //[Space(4)]
    //#endregion

    //#region LADDER STACKING VARIABLES
    //[Header("LADDER STACKING VARIABLES")]
    //[SerializeField] protected bool isFallingFromLadder = false;
    //[SerializeField] private bool insideTrigger_LadderPlatform = false;
    //[SerializeField] private LadderPlatform[] designatedLadderPlatforms;
    //[SerializeField] protected ObjectPooler objectPooler;
    //[SerializeField] protected float ladderHeight;
    //[SerializeField] protected float stackingCorrectionDeltaY = 0.7f;
    //[SerializeField] protected float stackingCorrectionForward = 0.7f;
    //[SerializeField] private float ladderPlatformOverlapCapsuleRadius = 2f;
    //[SerializeField] private LayerMask ladderPlatformMask;
    //[Space(4)]
    //#endregion

    //#region BOMB RELATED VARIABLES
    //[Header("BOMB RELATED VARIABLES")]
    //[SerializeField] protected bool canTakeDirectBombExplosion = true;
    //[SerializeField] protected Bomb carriedBomb = null;
    //[SerializeField] protected bool isCarryingBomb = false;
    //[SerializeField] protected Trajectory trajectory;
    //private Transform bombCarryTransform;
    //[SerializeField] private Vector3 bombCarryLocalPos;
    //[SerializeField] protected Vector3 bombThrowVector = Vector3.up;
    //[SerializeField] protected float bombThrowSpeed = 3f;
    //[SerializeField] protected Vector3 bombThrowPos;
    //[Space(4)]
    //#endregion

    //#region ANIMATION VARIABLES
    //[Header("ANIMATION VARIABLES")]
    //private int isIdleBoolHash;
    //private int isIdleCarryingBoolHash;
    //private int isRunEmptyBoolHash;
    //private int isRunCarryingBoolHash;
    //private int isKnockedDownBoolHash;
    //private int isClimbingBoolHash;
    //private int isFallingBoolHash;
    //private int isClimbingTopTreeBoolHash;
    //private int podiumPoseTriggerHash;
    //private int idleTriggerHash;
    //#endregion
}
