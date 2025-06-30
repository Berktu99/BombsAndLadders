using UnityEngine;
using DG.Tweening;

public class PlayerController : Humanoid
{
    [MyBox.Foldout("Player Variables",true)]
    [SerializeField] private GameObject crosshairTarget;
    [SerializeField] private Joystick joystick;

    public StringVariable playerName;

    public IntVariable characterSkinActiveIndex;
    public IntVariable colorSkinActiveIndex;

    public HumanoidSkins characterSkins;
    public ColorSkins colorSkins;
    public IntVariable goldCount;

    private Vector3 beginPos;

    [SerializeField] private float ladderClimbAngleCheck = -0.7f;

    public override void Awake()
    {
        crosshairTarget = Instantiate(crosshairTarget);

        if (joystick == null)
        {
            joystick = FindObjectOfType<Joystick>(true);
        }

        base.Awake();
        crosshairTarget.transform.localScale = Vector3.zero;
        joystick.PointerLetGo += joystickLetGo;
    }

    public override void Start()
    {
        base.Start();
        matchPlayerWithSaveFile();

        beginPos = transform.position;
    }

    private void matchPlayerWithSaveFile()
    {
        equipNewCharacterSkin(characterSkins.availableSkins[characterSkinActiveIndex.Value].skinType, characterSkins.availableSkins[characterSkinActiveIndex.Value].skinPrefab);

        equipNewColorSkin(colorSkins.availableSkins[colorSkinActiveIndex.Value].skinColor);

        nameTag.GetComponentInChildren<TMPro.TextMeshPro>().text = playerName.Value;

        goldCount.SetValue(0);
    }

    public override void handleMovement()
    {
        if (humanoidMovementIsStopped)
        {
            isMovementPressed = false;
            currentMovement = Vector3.zero;

            return;
        }

        if (isKnockedDown || isClimbingTopTree || isEliminated)
        {
            return;
        }

        if (isONtheLadder)
        {
            if (joystick.Vertical > 0.1)
            {
                climbingDirection = 1;
            }
            else if(joystick.Vertical < -0.1)
            {
                climbingDirection = -1;
            }
            else
            {
                climbingDirection = 0;
            }

            return;
        }       

        isMovementPressed = Helpers.performantMagnitudeCompare(joystick.Direction, 0.05f);

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes
            currentMovement = new Vector3(joystick.Horizontal, 0.0f, joystick.Vertical);
            currentMovement *= speed * -1f;
        }


        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        //currentMovement.y += gravity * Time.unscaledDeltaTime;

        // Move the controller
        characterController.Move(currentMovement * Time.unscaledDeltaTime);
    }

    public override void handleRotation()
    {
        if (isKnockedDown || isONtheLadder || isFallingFromLadder || isEliminated)
        {
            return;
        }

        Vector3 positionToLookAt = Vector3.zero;

        positionToLookAt.x = joystick.Horizontal * -1f;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = joystick.Vertical * -1f;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed && characterController.isGrounded)
        {
            if (positionToLookAt == Vector3.zero)
            {
                Debug.Log("zero");
            }

            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    public override bool makeHumanPickUpBomb(Bomb bomb)
    {        
        if (base.makeHumanPickUpBomb(bomb))
        {
            crosshairTarget.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBounce);
            return true;
        }
        return false;
    }

    private void joystickLetGo()
    {
        if (isCarryingBomb)
        {
            throwBomb();
        }
    }

    public override void throwBomb()
    {
        crosshairTarget.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InElastic);
        Vector3 vel = new Vector3(transform.forward.x, bombThrowVector.y, transform.forward.z);
        vel.Normalize();
        carriedBomb.getThrown(vel * bombThrowSpeed, afterThrowBombLockTime);
        carriedBomb = null;

        isCarryingBomb = false;
    }

    public override void handleCollisionChecks()
    {
        handleLadderPlatformCheck();
        void handleLadderPlatformCheck()
        {
            //Collider[] ladderCollider = Physics.OverlapSphere(sphereCastPos.position, ladderPlatformOverlapCapsuleRadius, ladderPlatformMask, QueryTriggerInteraction.Collide);
            ladderColliderLength = Physics.OverlapSphereNonAlloc(selfTransform.position, ladderPlatformOverlapCapsuleRadius, ladderCollider, ladderPlatformMask, QueryTriggerInteraction.Collide);

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
                            checkClimbCondiiton(true);
                        }
                    }
                    else
                    {
                        if (lp.compareKey(key))
                        {
                            checkClimbCondiiton(false);
                        }
                    }

                    void checkClimbCondiiton(bool assign)
                    {
                        if (Vector3.Dot(lp.gameObject.transform.forward.normalized, this.transform.forward.normalized) < ladderClimbAngleCheck)
                        {
                            if (assign)
                            {
                                lp.assignHumanoid(pickUpVariables);
                                beginClimb();
                            }
                            else
                            {
                                beginClimb();
                            }
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

                        currentMovement = Vector3.zero;

                        characterController.Move(lp.transform.GetChild(2).position + 1.2f * Vector3.up - this.transform.position);

                        transform.eulerAngles = lp.transform.GetChild(2).eulerAngles;
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
            
            pickUpHitColliders_handleColCheck_Length = Physics.OverlapSphereNonAlloc(selfTransform.position, pickUpOverlapCapsuleRadius, pickUpHitColliders_handleColCheck, layerMaskPickUp, QueryTriggerInteraction.Collide);

            if (pickUpHitColliders_handleColCheck_Length > 0)
            {
                if (pickUpHitColliders_handleColCheck[0].TryGetComponent<PickUp>(out PickUp t))
                {
                    if (t.compareKey(key))
                    {
                        t.makeThisPickUpPickedUp(pickUpPackTransform, thisHumanoidMaterial, key);                        

                        currentPickUpCount++;

                        onPickUpPickedUp.Raise();
                        goldCount.SetValue(goldCount.Value + 1);
                    }
                }
            }

            currentPickUpCount = pickUpPackTransform.childCount;
        }

        handleGroundCheck();
        void handleGroundCheck()
        {
            //Collider[] groundHit_handleColCheck = Physics.OverlapBox(sphereCastPos.position, new Vector3(0.01f, 1f, 0.01f), Quaternion.identity, groundMask, QueryTriggerInteraction.Collide);//Physics.OverlapSphere(sphereCastPos.position, 0.5f, groundMask, QueryTriggerInteraction.Collide);

            groundHit_handleColCheck_Length = Physics.OverlapSphereNonAlloc(selfTransform.position, 0.1f, groundHit_handleColCheck, groundMask, QueryTriggerInteraction.Collide);//Physics.OverlapSphere(sphereCastPos.position, 0.5f, groundMask, QueryTriggerInteraction.Collide);

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
        }
    }
    
    public void revivePlayer()
    {
        isEliminated = false;
        transform.position = beginPos;
        startHumanoidMovement();
    }

    public override void tryGetCaughtInBombExplosion(Humanoid humanoidBombedThis)
    {
        if (canTakeDirectBombExplosion)
        {
            getKnockDown();

            lastDamager = new Damager(humanoidBombedThis.Key, humanoidBombedThis.ThisHumanoidMaterial);
            registerDmgTimer = registerDmgTime;

            canTakeDirectBombExplosion = false;

            isCarryingBomb = false;

            crosshairTarget.transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InElastic);

            return;
        }
    }

    public override void handleBombTrajectory()
    {
        base.handleBombTrajectory();
        crosshairTarget.transform.position = bombThrowPos + Vector3.up * 0.05f;       
    }
}
