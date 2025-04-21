using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistAttack1 : PlayerBaseState
{
    public FistAttack1(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine) : base(combatFactory, stateMachine) { }
    public override void OnStateEnter()
    {
        //PlayerManager.instance.playerMovementManager.MoveTowardsPosition(stateMachine.transform.position + (stateMachine.transform.forward * 3), stateMachine.transform.rotation, 0.2f);
        PlayerManager.instance.playerAnimationManager.PlayActionAnimation("LeftPunch", true, true, true, false, false, true);
    }
    public override void OnStateUpdate()
    {
        if (stateMachine.canAttack && PlayerInputManager.instance.attackPressed && !PlayerManager.instance.isPerformingAction)
        {
            
            stateMachine.SwitchState(combatFactory.FistAttack2());
        }
    }
    public override void OnStateExit()
    {

    }

}
