using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PickUp : MonoBehaviour, IamTarget
{
    [Header("General Variables")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform poolParent = null;
    [SerializeField] private Renderer thisRenderer = null;
    [SerializeField] private Rigidbody thisRigidbody = null;
    [SerializeField] private Collider thisCollider = null;  
    [SerializeField] private Material originalMaterial;
    [SerializeField] private int originalKey = 20;
    [SerializeField] private int originalLayer;
    [SerializeField] private int dePooledFloor = -1;
    [SerializeField] private int dePooledIndex = -1;

    [Space(3)]

    [Header("Check For Trigger Collision With Humanoid")]

    [SerializeField] private float overlapCapsuleRadius = 2f;
    [SerializeField] private LayerMask layerMaskHumanoid;
    [SerializeField] private int currentKey = 20;
    [SerializeField] private bool thisCanBePickedUp;
    

    [Space(3)]

    [Header("After Humanoid Trigger Variables")]

    [SerializeField] private float pickUpTweenTime = 0.5f;
    [SerializeField] private float gfxHeight = 0.2f;
    private Sequence moveSequence, rotateSequence;

    [Space(3)]

    [Header("Greyed Out Variables")]
    [SerializeField] private Material greyMaterial;
    [SerializeField] private int ignoreHumanoid;
    [SerializeField] private LayerMask groundCheck;
    [SerializeField] private float greyedOutTime = 4f;
    [SerializeField] private bool isEliminated_Suicide = false;
    private float greyedOutTimer;
    

    public PickUpSkins pickUpSkins;
    public IntVariable pickUpSkinActiveIndex;

    public PickUpPickedUpEvent onPickUpPickedUp;
    private void Awake()
    {
        levelManager = LevelManager.getInstance();
    }

    private void Update()
    {
        checkForHumanoid();        
    }

    private void checkForHumanoid()
    {
        if (currentKey == -2)
        {
            if (thisCanBePickedUp)
            {
                greyedOutTimer -= Time.unscaledDeltaTime;
                if (greyedOutTimer <= 0f)
                {
                    rePool();
                }
                else
                {
                    checkForHumanoid_Grey();
                }
            }                    
        }        


        void checkForHumanoid_Grey()
        {
            if (!thisCanBePickedUp) return;

            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, overlapCapsuleRadius, layerMaskHumanoid);            

            if (hitColliders.Length <= 0)
            {
                return;
            }

            List<Collider> availableHumanoids = new List<Collider>();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].GetComponent<Humanoid>().pickUpVariables.canCollectPickUp)
                {
                    availableHumanoids.Add(hitColliders[i]);
                }
            }

            if (availableHumanoids.Count <= 0)
            {
                return;
            }

            int closestColliderIndex = 0;
            float closestDist = float.MaxValue;

            for (int i = 1; i < availableHumanoids.Count; i++)
            {
                float compareTo = Helpers.performantDistance(availableHumanoids[i].transform.position, this.transform.position);
                if (compareTo < closestDist)
                {
                    closestColliderIndex = i;
                    closestDist = compareTo;
                }
            }

            HumanoidPickUpVariables humanoidPickUpVariables = availableHumanoids[closestColliderIndex].GetComponent<Humanoid>().pickUpVariables;

            if (humanoidPickUpVariables.canCollectPickUp)
            {
                makeThisPickUpPickedUp(humanoidPickUpVariables.pickUpPackTr, 
                    humanoidPickUpVariables.mat, 
                    humanoidPickUpVariables.keyValue);
            }
        }
    } 

    public void makeThisPickUpPickedUp(Transform pickUpPackTr, Material humanoidMaterial, int key)
    {
        if (isEliminated_Suicide)
        {
            // now this pick up belongs to whoever picks it up after
            // designated humanoid of this is eliminated.
            originalKey = key;
            originalMaterial = humanoidMaterial;
            isEliminated_Suicide = false;
            poolParent = poolParent.parent.GetChild(key);
        }

        onPickUpPickedUp.Raise(new PickUpPickedUp(dePooledFloor, dePooledIndex));
        
        //levelManager.floors[dePooledFloor].cells[dePooledIndex].occupied = false;
        //levelManager.allFloorsUnoccupiedCellIndexes[dePooledFloor].unoccupiedCellIndexes.Add(dePooledIndex);

        int indexInPack = pickUpPackTr.childCount;

        thisRigidbody.isKinematic = true;
        thisCollider.enabled = false;
        thisRenderer.sharedMaterial = humanoidMaterial;      

        currentKey = key;

        thisCanBePickedUp = false;

        moveSequence = DOTween.Sequence();
        moveSequence.Append(transform.DOLocalMove(Vector3.up * gfxHeight * indexInPack, pickUpTweenTime));

        rotateSequence = DOTween.Sequence();
        rotateSequence.Append(transform.DOLocalRotateQuaternion(Quaternion.identity, pickUpTweenTime));

        this.transform.parent = pickUpPackTr.transform;
        //StartCoroutine(waitFor());
        //IEnumerator waitFor()
        //{
        //    maybe look into this pls,

        //    yield return new WaitForSeconds(pickUpTweenTime);

        //    this.transform.parent = pickUpPackTr.transform;
        //}
    }

    public void simulateFalling()
    {
        this.transform.parent = poolParent;

        moveSequence.Kill();
        rotateSequence.Kill();

        thisCollider.enabled = true;
        thisCollider.isTrigger = false;

        thisRigidbody.isKinematic = false;
        //GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(0, 1)) * randomForceStrength);

        thisRenderer.sharedMaterial = greyMaterial;

        thisCanBePickedUp = false;

        gameObject.layer = ignoreHumanoid;

        StartCoroutine(checkGrounded());
        IEnumerator checkGrounded()
        {
            while (!Physics.Raycast(transform.position, Vector3.down, gfxHeight + 0.05f, groundCheck))
            {
                //yield return Yielders.CachedWaitForSeconds(Time.unscaledDeltaTime);
                //yield return new WaitForSeconds(Time.unscaledDeltaTime);
                yield return null;
            }

            // can be picked up by any humanoid now
            currentKey = -2;
            thisCanBePickedUp = true;

            gameObject.layer = originalLayer;

            greyedOutTimer = greyedOutTime;
        }
    }

    public bool compareKey(int key)
    {
        if (key == currentKey || currentKey == -1)
        {
            return true;
        }
        return false;
    }

    public void dePool(int dePooledFloor, int dePooledIndex, Vector3 dePoolPos, Quaternion rotation, int unoccpiedIndex)
    {
        //gfxTransform.gameObject.SetActive(true);

        thisCanBePickedUp = true;

        this.dePooledFloor = dePooledFloor;
        this.dePooledIndex = dePooledIndex;

        //levelManager.floors[dePooledFloor].cells[dePooledIndex].occupied = true;
        //levelManager.allFloorsUnoccupiedCellIndexes[dePooledFloor].unoccupiedCellIndexes.RemoveAt(unoccpiedIndex);

        //this.transform.localRotation = Quaternion.identity;

        //this.transform.position = dePoolPos;
        this.transform.SetPositionAndRotation(dePoolPos, rotation);

        StartCoroutine(waitFor());
        IEnumerator waitFor()
        {
            yield return Yielders.CachedWaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.1f);

            thisRigidbody.isKinematic = true;
            thisCollider.enabled = true;
        }
    }

    public void rePoolEliminated(int key, Material mat)
    {
        moveSequence.Kill();
        rotateSequence.Kill();

        levelManager.allHumanoidRepooledPickUps[key].repooledPickUps.Add(this);

        this.transform.parent = poolParent;
        this.transform.localPosition = Vector3.zero;

        thisRigidbody.isKinematic = true;
        thisCollider.enabled = false;

        thisRenderer.sharedMaterial = mat;

        this.transform.localRotation = Quaternion.identity;        

        currentKey = key;
        gameObject.layer = originalLayer;

        originalKey = key;
        originalMaterial = mat;

        poolParent = poolParent.parent.GetChild(key);
    }

    public void rePoolEliminated()
    {
        moveSequence.Kill();
        rotateSequence.Kill();

        levelManager.allHumanoidRepooledPickUps[currentKey].repooledPickUps.Add(this);

        this.transform.parent = poolParent;
        this.transform.localPosition = Vector3.zero;

        thisRigidbody.isKinematic = true;
        thisCollider.enabled = false;

        thisRenderer.sharedMaterial = greyMaterial;

        this.transform.localRotation = Quaternion.identity;

        currentKey = -1;
        gameObject.layer = originalLayer;

        isEliminated_Suicide = true;
    }

    public void greyOutLadderPickUp(int key, Material mat)
    {
        originalKey = key;
        originalMaterial = mat;

        poolParent = poolParent.parent.GetChild(key);
    }

    public void greyOutLadderPickUp()
    {        
        currentKey = -1;
        thisRenderer.sharedMaterial = greyMaterial;

        isEliminated_Suicide = true;
    }

    public void rePool()
    {
        //if (!this.gameObject.activeSelf)
        //{
        //    this.gameObject.SetActive(true);
        //}

        moveSequence.Kill();
        rotateSequence.Kill();

        levelManager.allHumanoidRepooledPickUps[originalKey].repooledPickUps.Add(this);

        this.transform.parent = poolParent;
        this.transform.localPosition = Vector3.zero;


        thisRigidbody.isKinematic = true;
        thisCollider.enabled = false;

        thisRenderer.sharedMaterial = originalMaterial;
        //gfxTransform.gameObject.SetActive(false);

        this.transform.localRotation = Quaternion.identity;

        currentKey = originalKey;
        gameObject.layer = originalLayer;
        gameObject.SetActive(false);
    }    

    public void initializePickUpValues(int key, Material humanoidMaterial)
    {
        originalLayer = gameObject.layer;

        originalKey = key;
        currentKey = key;

        poolParent = this.transform.parent;

        thisRenderer = GetComponentInChildren<Renderer>();
        thisRigidbody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();

        thisRenderer.sharedMaterial = humanoidMaterial;
        originalMaterial = humanoidMaterial;

        gfxHeight = pickUpSkins.availableSkins[pickUpSkinActiveIndex.Value].pickUpSkinHeight;

        thisRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
