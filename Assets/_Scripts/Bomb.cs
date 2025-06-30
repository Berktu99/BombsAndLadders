using UnityEngine;
using System.Collections;


public class Bomb : MonoBehaviour, IMustInitialize, IamTarget
{
    [Header("Pick-Up Variables")]
    [SerializeField] private LayerMask humanoidMask;
    [SerializeField] private Transform poolParentTransform;
    [SerializeField] private float overlapCapsuleRadius;
    public bool pickUpAble = true;
    [SerializeField] private SpriteRenderer pickUpAbleity;
    [SerializeField] private Humanoid lastHumanoidPickedThis = null;
    public bool pickedUpFromPadestal = false;

    [Space(2)]

    [Header("Explosion Variables.")]
    [SerializeField] private LayerMask affectedByExplosion;    
    [SerializeField] private float explodeRadius = 7f;
    [SerializeField] private float explodeTime = 4f;
    [SerializeField] private bool isExplodeCountdown = false;
    [SerializeField] private int hitboxEffectiveFrames;
    public bool currentlyPickedUp = false;

    private CountdownTimer countdownTimer;
    
    [SerializeField] private GameObject bombGFX;
    [Space(2)]

    [Header("Animation Variables.")]
    public Color32 startColor;
    public Color32 endColor;
    public float speed = 0.3f;
    public float speedAccel = 1f;
    public Renderer pulse;
    public ParticleSystem boomPS;

    private void Start()
    {
        hitColliders = new Collider[FindObjectsOfType<Humanoid>().Length];

        pulse = gameObject.FindComponentInChildWithTag<Renderer>("BombPulse");
        pulse.gameObject.SetActive(false);

        countdownTimer = new CountdownTimer(explodeTime);

        pickUpAbleity.enabled = false;
    }

    public void Initialize()
    {
        poolParentTransform = this.transform.parent;
        pickUpAbleity = GetComponentInChildren<SpriteRenderer>();
        managePickUpLockGFX();       
    }

    private void Update()
    {
        checkForHumanPickUpTrigger();
    }

    protected Collider[] hitColliders;
    private Humanoid h;
    private void checkForHumanPickUpTrigger()
    {
        if (pickUpAble)
        {
            int c = Physics.OverlapSphereNonAlloc(this.transform.position, overlapCapsuleRadius, hitColliders, humanoidMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < c; i++)
            {
                h = hitColliders[i].GetComponent<Humanoid>();

                currentlyPickedUp = h.makeHumanPickUpBomb(this);

                if (currentlyPickedUp == true)
                {
                    if (!pickedUpFromPadestal)
                    {
                        pickedUpFromPadestal = true;
                    }

                    pickUpAbleity.enabled = false;
                    if (!pulse.gameObject.activeSelf)
                    {
                        pulse.gameObject.SetActive(true);
                    }

                    lastHumanoidPickedThis = h;
                    if (!isExplodeCountdown)
                    {
                        StartCoroutine(doCountdownRoutine());
                        isExplodeCountdown = true;
                    }

                    managePickUpLockGFX();

                    break;
                }
            }
        }
    }

    IEnumerator doCountdownRoutine()
    {
        float timer = 0;
        float pulseSpeed = 0;

        while (timer < explodeTime)
        {
            pulse.material.SetFloat("_Speed", pulseSpeed);

            pulseSpeed += Time.unscaledDeltaTime * speed;

            timer += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }

        exlode();
    }

    private void exlode()
    {
        this.transform.parent = poolParentTransform;
        boomPS.gameObject.SetActive(true);
        boomPS.Play();
        bombGFX.SetActive(false);
        GetComponent<Collider>().enabled = false;
        pickUpAble = true;

        StartCoroutine(explosionRadiusCheck());
        StartCoroutine(doRepoolRoutine());
    }

    private IEnumerator explosionRadiusCheck()
    {
        int frameCount = hitboxEffectiveFrames;
        while (frameCount >= 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, explodeRadius, affectedByExplosion, QueryTriggerInteraction.Collide);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<Humanoid>(out Humanoid h))
                {
                    h.tryGetCaughtInBombExplosion(lastHumanoidPickedThis);
                }
                else if (hitCollider.TryGetComponent<LadderPlatform>(out LadderPlatform lp))
                {
                    lp.tryExplodeLadders();
                }
                else
                {
                    Debug.Log("You have a mismatch in layers and humanoid scripts.");
                }
            }

            frameCount--;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
    }

    IEnumerator doRepoolRoutine()
    {
        while (boomPS != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!boomPS.IsAlive(true))
            {
                repool();
                    
                break;
            }
        }
    }

    private void repool()
    {
        boomPS.Clear();
        GetComponent<Rigidbody>().isKinematic = true;
        transform.localPosition = Vector3.zero;
        currentlyPickedUp = false;        
        bombGFX.SetActive(true);
        this.transform.parent = poolParentTransform;
        GetComponent<Collider>().enabled = true;
        pickUpAble = true;
        managePickUpLockGFX();

        StopAllCoroutines();

        pulse.gameObject.SetActive(false);
        pickedUpFromPadestal = false;
        pickUpAbleity.enabled = false;

        this.gameObject.SetActive(false);
    }

    public void getThrown(Vector3 throwSpeed, float lockTime)
    {
        Vector3 a = transform.eulerAngles;
        a.x = 0f;
        a.z = 0f;
        transform.eulerAngles = a;

        this.transform.parent = poolParentTransform;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().linearVelocity = throwSpeed;

        pickUpAbleity.enabled = true;

        pickUpAble = false;

        managePickUpLockGFX();

        float sec = 0.3f;

        StartCoroutine(waitFor(sec));        
        IEnumerator waitFor(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            currentlyPickedUp = false;
        }

        StartCoroutine(waitFor2(lockTime));
        IEnumerator waitFor2(float seconds)
        {
            yield return new WaitForSeconds(seconds);            
            pickUpAble = true;
            managePickUpLockGFX();
        }
    }    

    private void managePickUpLockGFX()
    {
        if (pickUpAble)
        {
            pickUpAbleity.color = Color.green;
        }
        else
        {
            pickUpAbleity.color = Color.red;
        }

        if (currentlyPickedUp)
        {
            pickUpAbleity.enabled = false;
        }
        else
        {
            pickUpAbleity.enabled = true;
        }
    }
}
