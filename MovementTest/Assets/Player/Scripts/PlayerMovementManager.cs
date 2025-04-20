using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class PlayerMovementManager : MonoBehaviour
{
    [Header("Auto-Initizalized References")]
    public PlayerManager playerManager;
    public PlayerInputManager playerInputManager;

    [Header("Movement Attributes")]
    public float speedAcceleration;
    public float currentSpeed;
    public float maxSprintSpeed;
    public float maxRunSpeed;
    public float maxWalkSpeed;
    public float rootMotionSpeedMultiplierXZ = 1;
    public float rootMotionSpeedMultiplierY = 1;
    public float airGravityScale = -5f;
    public float groundGravityScale = -9.81f;
    public Vector3 verticalVelocity;
    public Vector3 XZvelocity;

    [Header("Rotation Attributes")]
    public float rotationSlerpSpeed;
    public Vector3 playerDirection;

    [Header("Action Flags")]
    public bool canRotate = true;
    public bool canMove = true;
    public bool useGravity = true;

    [Header("Jumping Settings")]
    public float jumpingHeight;
    public bool isJumping = false;
    private float timeAboveGround;
    private bool fallingWithoutJump = false;



    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        playerInputManager = PlayerInputManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGroundedMovements();
        HandleMovementRotations();
        HandleGravity();
        HandleJumping();
        CalculatePlayerInputDirection();
    }

    private void CalculatePlayerInputDirection()
    {
        playerDirection = playerManager.mainCam.transform.right * playerInputManager.movementDirection.x;
        playerDirection += playerManager.mainCam.transform.forward * playerInputManager.movementDirection.y;

        playerDirection.y = 0;
        playerDirection.Normalize();
    }
    public void HandleGroundedMovements()
    {
        if (!canMove)
            return;

        if (!playerManager.animator.applyRootMotion && playerInputManager.movementDirection != Vector2.zero)
        {

           
            float speedCap = 0f;

            if (playerInputManager.moveAmount > 1f)
            {
                speedCap = maxSprintSpeed;
            }
            else if (playerInputManager.moveAmount > 0.5f)
            {
                speedCap = maxRunSpeed;
            }
            else 
            {
                speedCap = maxWalkSpeed;
            }

            if (currentSpeed < speedCap)
            {
                currentSpeed += speedAcceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, speedCap, speedAcceleration * Time.deltaTime);
            }
 
        }
        else if (playerManager.isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, speedAcceleration * Time.deltaTime);
        }


        // HandleJumping(ref velocity);

        playerManager.characterController.Move(playerDirection * currentSpeed * Time.deltaTime);  
    }


    public void HandleGravity()
    {
        if (!useGravity)
            return;


        if (verticalVelocity.y < 0 && !isJumping)
        {
            verticalVelocity.y = groundGravityScale;
        }
        else
        {

            verticalVelocity.y += airGravityScale * Time.deltaTime;
        }

        
        playerManager.characterController.Move(verticalVelocity * Time.deltaTime);


        // Reset to origin state on landing
        if (playerManager.isGrounded && ((isJumping && verticalVelocity.y < 0) || fallingWithoutJump))
        {
            isJumping = false;
            fallingWithoutJump = false;
            playerManager.animator.CrossFade("MovementBlend", 0.1f);
        }


        timeAboveGround = playerManager.isGrounded ? 0 : timeAboveGround + Time.deltaTime;



        // Start jump loop if falling too long without jumping
        if (!playerManager.isGrounded && !isJumping && timeAboveGround > 0.2f && !fallingWithoutJump)
        {
            playerManager.animator.CrossFade("JumpCycle", 0.1f);
            fallingWithoutJump = true;  
        }
    }

    public void HandleJumping()
    {
        if (playerManager.isGrounded && playerInputManager.jumpPressed)
        {
            playerManager.playerAnimationManager.PlayActionAnimation("JumpUp", true, false, false, true, true, true);
        }
    }

    public void ApplyJumpForce()
    {
        verticalVelocity.y = Mathf.Sqrt(-2 * airGravityScale * jumpingHeight);
        isJumping = true;
    }

    public void HandleMovementRotations()
    {
        if (!canRotate)
            return;

        if (playerInputManager.movementDirection != Vector2.zero)
        {

            if (playerDirection != Vector3.zero)
            {
                Quaternion desiredDirection = Quaternion.LookRotation(playerDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredDirection, rotationSlerpSpeed * Time.deltaTime);
            }
            
        }
        else
        {
            playerDirection = Vector3.zero;
        }
    }

    public IEnumerator MoveTowardsPosition(Vector3 position, Quaternion rotation, float duration)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            Vector3 lerpedPosition = Vector3.Lerp(transform.position, position, timeElapsed / duration);

            Vector3 deltaMovement = lerpedPosition - transform.position;
            playerManager.characterController.Move(deltaMovement);

            if (rotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, timeElapsed / duration);
            }

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        playerManager.characterController.enabled = false;
        transform.position = position;
        playerManager.characterController.enabled = true;

        if (rotation != Quaternion.identity)
        {
            transform.rotation = rotation;
        }
    }

    public IEnumerator PushInDirection(Vector3 direction, float distance, float duration)
    {
        float timeElapsed = 0;
        direction.Normalize();

        float movementOverTime = distance / duration;

        while (timeElapsed < duration)
        {
            playerManager.characterController.Move(direction * movementOverTime * Time.deltaTime);

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }
}
