using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAnimationManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public PlayerManager playerManager;
    public Animator animator;
    Gamepad gamepad;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = PlayerInputManager.instance;
        playerManager = PlayerManager.instance;
        animator = playerManager.animator;

        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAnimationMovementParameters(float horizontal, float vertical, float moveAmount)
    {
        animator.SetBool("isMoving", PlayerInputManager.instance.movementDirection != Vector2.zero);
        if (!playerManager.playerMovementManager.canMove)
            return; 

        animator.SetFloat("horizontal", horizontal, 1f, Time.deltaTime * 6f);
        animator.SetFloat("vertical", vertical, 1f, Time.deltaTime * 6f);
        
        
    }

    public void PlayActionAnimation(
        string animationName, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool rotateTowardsPlayerInput = false,
        bool canRotate = false,
        bool canMove = false,
        bool useGravity = true)
    {

        if (rotateTowardsPlayerInput && playerInputManager.movementDirection != Vector2.zero)
        {
            Vector3 playerDirection = playerManager.mainCam.transform.right * playerInputManager.movementDirection.x;
            playerDirection += playerManager.mainCam.transform.forward * playerInputManager.movementDirection.y;

            playerDirection.y = 0;
            playerDirection.Normalize();

            StartCoroutine(SlerpDuringAction(Quaternion.LookRotation(playerDirection), 0.5f));
            //playerManager.transform.rotation = Quaternion.LookRotation(playerDirection);
        }
        animator.CrossFade(animationName, 0.1f);
        playerManager.isPerformingAction = isPerformingAction;
        playerManager.playerMovementManager.canMove = canMove;
        playerManager.playerMovementManager.canRotate = canRotate;
        animator.applyRootMotion = applyRootMotion;
        playerManager.playerMovementManager.useGravity = useGravity;
    }

    public IEnumerator SlerpDuringAction (Quaternion rotation, float dampTime)
    {
        float timer = 0;

        while (timer < dampTime)
        {
            playerManager.transform.rotation = Quaternion.Slerp(playerManager.transform.rotation, rotation, timer / dampTime);
            timer += Time.deltaTime;
            yield return null;
        }

    }

    private void OnAnimatorMove()
    {
        if (animator.applyRootMotion)
        {
            
            Vector3 velocity = animator.deltaPosition;
            velocity.x *= playerManager.playerMovementManager.rootMotionSpeedMultiplierXZ;
            velocity.z *= playerManager.playerMovementManager.rootMotionSpeedMultiplierXZ;
            velocity.y *= playerManager.playerMovementManager.rootMotionSpeedMultiplierY;



            if (playerManager.playerMovementManager.useGravity)
                velocity.y += playerManager.playerMovementManager.airGravityScale * Time.deltaTime;

            playerManager.characterController.Move(velocity);
        }
    }

    public void HitStop()
    {
        StartCoroutine(ProcessHitStop(0.04f));

        if (gamepad != null)
            gamepad.SetMotorSpeeds(0.1f, 0.05f);
    }

    public IEnumerator ProcessHitStop(float time)
    {
        animator.speed = 0;

        yield return new WaitForSeconds(time);

        animator.speed = 1;
    }

    #region AnimationEvents
    public void PerformingActionTrue()
    {
        playerManager.isPerformingAction = true;
    }
    public void PerformingActionFalse()
    {
        playerManager.isPerformingAction = false;
    }

    public void ActivateGravity(float scale)
    {
        playerManager.playerMovementManager.groundGravityScale = scale;
        playerManager.playerMovementManager.groundGravityScale = scale;
        playerManager.playerMovementManager.useGravity = true;
    }

    /*public void CanAttackTrue()
    {
        playerManager.canAttack = true;
    }

    public void CanAttackFalse()
    {
        playerManager.canAttack = false; 
    }

    public void OpenWeaponCollider()
    {
        playerManager.weaponCollider.enabled = true;
    }

    public void CloseWeaponCollider()
    {
        playerManager.weaponCollider.enabled = false;
        playerManager.currentEquippedWeapon.ClearDamagedTargetList();
    }*/
    #endregion
}
