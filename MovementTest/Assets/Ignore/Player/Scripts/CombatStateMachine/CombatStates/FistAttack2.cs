using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistAttack2 : PlayerBaseState
{
    public FistAttack2(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine) : base(combatFactory, stateMachine) { }
    public override void OnStateEnter()
    {
        PlayerManager.instance.playerAnimationManager.PlayActionAnimation("RightPunch", true, true, true, false, false, true);
    }
    public override void OnStateUpdate()
    {
        if (stateMachine.canAttack && PlayerInputManager.instance.attackPressed && !PlayerManager.instance.isPerformingAction)
        {

            stateMachine.SwitchState(combatFactory.Kick1());
        }
    }
    public override void OnStateExit()
    {

    }

}
