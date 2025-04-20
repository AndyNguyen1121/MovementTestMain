using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Assign in Inspector")]
    public Animator animator;
    public PlayerAnimationManager playerAnimationManager;
    public CharacterController characterController;

    [Header("Auto-Initizalized References")]
    public PlayerMovementManager playerMovementManager;
    public Camera mainCam;

    [Header("Action Flags")]
    public bool isPerformingAction = false;
    public bool canAttack = true;

    [Header("Rigs")]
    public Rig shieldRig;

    [Header("Blood Particle")]
    public GameObject bloodParticle;

    [Header("Ground Check")]
    public bool isGrounded = true;
    public float groundCheckRadius;
    public Vector3 groundCheckOffset;
    public LayerMask whatIsGround;

    public Collider[] colliders;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCam = Camera.main;
        playerMovementManager = GetComponent<PlayerMovementManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //IgnoreMyOwnColliders();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundedState();

    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
            
    }

    public void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
       colliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in colliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
