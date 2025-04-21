using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetValues : StateMachineBehaviour
{

    public bool switchToIdleState = true;
    public bool isPerformingAction;
    public bool canMove;
    public bool canRotate;
    public bool useGravity;
    public bool applyRootMotion;

    public float rootMotionSpeedXZ;
    public float rootMotionSpeedY;
    public float defaultGravity = -9.81f;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager playerManager = animator.GetComponent<PlayerManager>();

        if (playerManager != null)
        {
            if (switchToIdleState)
            {
                playerManager.playerCombatStateMachine.SwitchState(playerManager.playerCombatStateMachine.combatFactory.CombatIdle());
            }

            playerManager.isPerformingAction = this.isPerformingAction;
            animator.applyRootMotion = this.applyRootMotion;
            playerManager.playerMovementManager.canMove = this.canMove;
            playerManager.playerMovementManager.canRotate = this.canRotate;
            playerManager.playerMovementManager.rootMotionSpeedMultiplierXZ = rootMotionSpeedXZ;
            playerManager.playerMovementManager.rootMotionSpeedMultiplierY = rootMotionSpeedY;  
            playerManager.playerMovementManager.useGravity = this.useGravity;
            playerManager.playerMovementManager.groundGravityScale = defaultGravity;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }


}
