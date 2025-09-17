using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerControls inputActions;
    public PlayerManager playerManager;
    public float deadZone = 0.2f;

    [Header("Movement")]
    public Vector2 movementDirection;
    public Vector2 clampedDirection;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;

    [Header("Press flags")]
    public bool sprintPressed = false;
    public bool isSprinting = false;
    public bool isInteracting = false;
    public bool jumpPressed = false;
    public bool attackPressed = false;

    [Header("Input Queue")]
    public float defaultQueueTimer = 0.35f;
    public float queueTimer = 0f;
    public bool queueIsActive = false;
    public bool attackInputQueue = false;

    // Assign in Inspector
    [Header("Camera Controls")]
    public CinemachineInputProvider cameraInputProvider;

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

        Application.targetFrameRate = 300;
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleSprintingInput();
       /* HandleInteractInput();
        HandleJumpingInput();
        HandleAttackInput();
        HandleQueuedInputs();*/
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();

            inputActions.MovementMap.Movement.performed += ctx => movementDirection = ctx.ReadValue<Vector2>();
            inputActions.MovementMap.Sprint.performed += ctx => sprintPressed = true;
            inputActions.MovementMap.Sprint.canceled += ctx => sprintPressed = false;
            inputActions.Actions.AttackQueue.performed += ctx => QueueInput(ref attackInputQueue);

        }

        inputActions.Enable();
        //cameraInputProvider.enabled = true;
    }

    private void OnDisable()
    {
        /*movementDirection = Vector2.zero;
        playerManager.playerAnimationManager.UpdateAnimationMovementParameters(0, 0, 0);
        cameraInputProvider.enabled = false;
        inputActions.Disable();*/
        
    }

    private void HandleInteractInput()
    {
        isInteracting = inputActions.Actions.Interact.WasPressedThisFrame();
    }

    private void HandleJumpingInput()
    {
        jumpPressed = inputActions.MovementMap.Jump.WasPressedThisFrame();
    }

    private void HandleSprintingInput()
    {
        if (sprintPressed && moveAmount > 0) 
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }
    private void HandleAttackInput()
    {
        attackPressed = inputActions.Actions.Attack.WasPressedThisFrame();
    }

    private void HandleMovementInput()
    {
        horizontalInput = movementDirection.x;
        verticalInput = movementDirection.y;

        float clampedHorizontal = 0f;
        float clampedVertical = 0f;


        if (Mathf.Abs(movementDirection.x) > 0.5)
        {
            clampedHorizontal = movementDirection.x > 0 ? 1 : -1f;
        }
        else if (Mathf.Abs(movementDirection.x) > deadZone)
        {
            clampedHorizontal = movementDirection.x > 0 ? 0.5f : -0.5f;
        }

        if (Mathf.Abs(movementDirection.y) > 0.5)
        {
            clampedVertical = movementDirection.y > 0 ? 1 : -1f;
        }
        else if (Mathf.Abs(movementDirection.y) > deadZone)
        {
            clampedVertical = movementDirection.y > 0 ? 0.5f : -0.5f;
        }

        moveAmount = Mathf.Clamp01(Mathf.Abs(clampedVertical) + Mathf.Abs(clampedHorizontal));

        if (moveAmount <= 0.5 && moveAmount > deadZone)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = isSprinting ? 1.5f : 1f;
        }
        else
        {
            moveAmount = 0;
        }

       
        playerManager.playerAnimationManager.UpdateAnimationMovementParameters(0, moveAmount, moveAmount);
        clampedDirection.x = clampedHorizontal;
        clampedDirection.y = clampedVertical;
    }

    private void QueueInput(ref bool queuedInput)
    {
        attackInputQueue = false;

        if (playerManager.isPerformingAction)
        {
            queuedInput = true;
            queueTimer = defaultQueueTimer;
            queueIsActive = true;
        }
    }

    private void ProcessQueuedInput()
    {
        if (attackInputQueue)
        {
            attackPressed = true;
        }
    }

    private void HandleQueuedInputs()
    {
        if (queueIsActive)
        {
            if (queueTimer > 0)
            {
                queueTimer -= Time.deltaTime;
                ProcessQueuedInput();
            }
            else
            {
                attackInputQueue = false;
                queueIsActive = false;
            }
        }
    }

}
