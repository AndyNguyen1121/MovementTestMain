using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngineInternal;

public class PlayerObstacleInteractions : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform footTransform;

    [Header("Auto-Initialized References")]
    public PlayerManager playerManager;
    public Animator animator;
    public Vector3 wallRunDir;

    #region WallRunVar
    [Header("Wall Running")]
    public bool isWallRunning = false;
    public bool isWallJumping = false;
    public LayerMask whatIsWallRunnable;
    public Vector3 wallRunDirection;
    public List<float> wallCooldowns = new();
    public float wallCooldownTimer;
    public float currentWallRunID;
    public bool wallRunRestrictionsSet = false;

    [Header("WallRun Motion Match")]

    #endregion

    #region VaultingVar
    [Header("Vaulting Check")]
    public int numOfSphereChecks;
    public float sphereRadius;
    public float distanceBetweenSpheres;
    public Vector3 sphereOffset;
    public LayerMask whatIsObstacle;
    public float rayDist;
    public bool[] spheres;
    public Vector3 startPos = Vector2.zero;

    [Header("Vault Target Match")]
    [SerializeField] public TargetMatchingData vaultTargetMatchingData;
    public float vaultingStartMatchTime = 0.13f;
    public float vaultEndMatchTime = 0.35f;
    public Coroutine attachToWallCoroutine;


    #endregion

    [Header("Debug")]
    public Vector3 ledgeTestVector = Vector3.zero;

    [Range(0, 5)]
    public float divideTime;

    [Range(0, 5)]
    public float distanceOffset;

    // Start is called before the first frame update
    void Start()
    {
        spheres = new bool[numOfSphereChecks];
        playerManager = PlayerManager.instance;
        animator = playerManager.animator;

        vaultTargetMatchingData = new TargetMatchingData();

    }

    // Update is called once per frame
    void Update()
    {

        HandleWallRunningAction();
        if (PlayerInputManager.instance.isInteracting && !playerManager.isPerformingAction)
        {
            HandleVaultingAction();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Vault"))
        {
            AttemptToTargetMatch(vaultTargetMatchingData);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WallRun"))
        {
            

        }
    }

    #region WallRunning
    private void HandleWallRunningAction()
    {
        if (PlayerInputManager.instance.jumpPressed && isWallRunning)
        {
            isWallRunning = false;
            isWallJumping = true;
            wallRunRestrictionsSet = false;
        }

        if (isWallJumping)
        {
            playerManager.playerAnimationManager.PlayActionAnimation("JumpUp", true, false, false, true, true, true);
            isWallJumping = false;
            StartCoroutine(BeginWallCooldown(currentWallRunID, wallCooldownTimer));
        }

        if (!isWallRunning)
        {
            currentWallRunID = -1;
            return;
        }

       
        if (!wallRunRestrictionsSet)
        {
            playerManager.playerAnimationManager.PlayActionAnimation("WallRun", true, true, false, false, false, false);
            wallRunRestrictionsSet = true;
        }


        Quaternion desiredRotation = Quaternion.LookRotation(wallRunDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, playerManager.playerMovementManager.rotationSlerpSpeed * Time.deltaTime);

        playerManager.characterController.Move(wallRunDir * playerManager.playerMovementManager.speedAcceleration * Time.deltaTime);
    }

    public void WallRunOnTriggerStay(Collider other)
    {
        if ((((whatIsWallRunnable & (1 << other.gameObject.layer)) != 0)) && !isWallRunning && playerManager.playerMovementManager.verticalVelocity.y < 0)
        {
            InitializeWallRunVariables(other);
        }
    }

    public void InitializeWallRunVariables(Collider other)
    {
        // return if Wall ID is already in list
        if (wallCooldowns.Contains(other.GetInstanceID()))
            return;

        Vector3 directionToFromPlayer = other.transform.position - transform.position;
        directionToFromPlayer.y = 0;
        RaycastHit hit;

        if (Physics.Raycast(footTransform.position, directionToFromPlayer, out hit, 5f, whatIsWallRunnable))
        {

            // wall to the left
            wallRunDir = Vector3.Cross(transform.up, hit.normal);

            if (Vector3.Dot(transform.forward, wallRunDir) < 0)
            {

                // wall to the right
                wallRunDir = -wallRunDir;
            }

            isWallRunning = true;

            float wallID = other.GetInstanceID();
            wallCooldowns.Add(wallID);
            currentWallRunID = wallID;

            // adjust position to match feet against wall
            Vector3 desiredPositionFromWall = hit.point + (hit.normal * 0.6f);
            desiredPositionFromWall.y = transform.position.y;

            Vector3 changeInPosition = desiredPositionFromWall - transform.position;
            Vector3 directionToPush = changeInPosition.normalized;
            directionToPush.y = 0;

            float distance = Vector3.Distance(desiredPositionFromWall, transform.position);

           // attachToWallCoroutine = StartCoroutine(playerManager.playerMovementManager.PushInDirection(hit.normal, 0.1f, 0.3f));

        }

        
    }

    

    public IEnumerator BeginWallCooldown(float ID, float wallCooldown)
    {
        yield return new WaitForSeconds(wallCooldown);

        wallCooldowns.Remove(ID);
    }

    #endregion

    #region Vaulting
    private void HandleVaultingAction()
    {
        Vector3 startingPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        float height = -1;
        GameObject vaultingObject = null;

        if (HandleVaultingCheck(ref startingPosition, ref endPosition, ref height, ref vaultingObject))
        {

            // Set root motion speed based on distance
            Vector3 playerPos = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 end = new Vector3(endPosition.x, 0f, endPosition.z);

            float distance = Vector3.Distance(playerPos, endPosition) + distanceOffset;

            float multiplier = (distance / divideTime);

            playerManager.playerMovementManager.rootMotionSpeedMultiplierXZ = multiplier;
            playerManager.playerMovementManager.rootMotionSpeedMultiplierY = 1f;

            playerManager.playerAnimationManager.PlayActionAnimation("Vault", true, true, false, false, false, false);

            // Target matching with ledge
            ledgeTestVector = new Vector3(startingPosition.x, height + 0.15f, startingPosition.z + 0.3f);

            vaultTargetMatchingData.matchPosition = ledgeTestVector;
            vaultTargetMatchingData.matchRotation = Quaternion.identity;
            vaultTargetMatchingData.target = AvatarTarget.LeftHand;
            vaultTargetMatchingData.weightMask = new MatchTargetWeightMask(new Vector3(1f, 1f, 1f), 0f);
            vaultTargetMatchingData.normalizedStartTime = vaultingStartMatchTime;
            vaultTargetMatchingData.normalizedEndTime = vaultEndMatchTime;
        }
    }

    private bool HandleVaultingCheck(ref Vector3 startingPosition, ref Vector3 endPos, ref float height, ref GameObject vaultObject)
    {
        bool canVault = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + sphereOffset, transform.forward, out hit, rayDist, whatIsObstacle))
        {

            int obstacleID = hit.collider.gameObject.GetInstanceID();

            startPos = hit.point;
            startingPosition = startPos;
            endPos = Vector3.zero;

            for (int i = 0; i < numOfSphereChecks; i++)
            {
                Collider[] hits = Physics.OverlapSphere((startPos + (transform.forward * distanceBetweenSpheres * i)) + sphereOffset, sphereRadius, whatIsObstacle);

                bool sameObjectDetected = false;

                foreach (Collider collider in hits)
                {
                    if (collider.gameObject.GetInstanceID() == obstacleID)
                    {
                        sameObjectDetected = true;
                        break;
                    }
                }

                if (!sameObjectDetected)
                {
                    endPos = startPos + (transform.forward * distanceBetweenSpheres * i) + sphereOffset;
                    canVault = true;
                    spheres[i] = true;

                    for (int j = 0; j < spheres.Length; ++j)
                    {
                        if (j == i)
                            continue;

                        spheres[j] = false;
                    }

                    break;
                }
                else
                {
                    spheres[i] = false;
                }


            }

            height = hit.collider.bounds.max.y;

            RaycastHit heightHitData;
            Vector3 heightOrigin = hit.point + Vector3.up * 10f;

            if (Physics.Raycast(hit.collider.transform.position, Vector3.down, out heightHitData, 10f, whatIsObstacle))
            {
                height = heightHitData.point.y;
            }

            vaultObject = hit.collider.gameObject;
        }
        else
        {
            startPos = Vector3.zero;
        }

        return canVault;
    }

    #endregion

    public void AttemptToTargetMatch(TargetMatchingData matchData)
    {
        if (animator.IsInTransition(0))
        {
            return;
        }

        float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

        if (normalizeTime > matchData.normalizedEndTime)
        {
            return;
        }

        if (!animator.isMatchingTarget)
        {
            animator.MatchTarget(matchData.matchPosition, matchData.matchRotation, matchData.target, matchData.weightMask, matchData.normalizedStartTime, matchData.normalizedEndTime);

            Debug.Log(matchData.matchPosition);
        }
        //playerManager.characterController.height = 1f;
        
    }
    private void OnDrawGizmos()
    {
        if (startPos != Vector3.zero)
        {
            for (int i = 0; i < numOfSphereChecks; i++)
            {
                if (spheres[i])
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireSphere((startPos + (transform.forward * distanceBetweenSpheres * i)) + sphereOffset, sphereRadius);
            }
        }

        if (ledgeTestVector != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(ledgeTestVector, sphereRadius);
        }

        Gizmos.DrawRay(transform.position, wallRunDir * 100);


        
    }
}
