using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinKick : PlayerBaseState
{
    public SpinKick(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine) : base(combatFactory, stateMachine) { }
    public override void OnStateEnter()
    {
        PlayerManager.instance.playerAnimationManager.PlayActionAnimation("SpinKick", true, true, true, false, false, true);
    }
    public override void OnStateUpdate()
    {
        /*if (stateMachine.canAttack && PlayerInputManager.instance.attackPressed && !PlayerManager.instance.isPerformingAction)
        {

            stateMachine.SwitchState(combatFactory.SpinKick());
        }*/
    }
    public override void OnStateExit()
    {

    }

}