using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick1 : PlayerBaseState
{
    public Kick1(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine) : base(combatFactory, stateMachine) { }
    public override void OnStateEnter()
    {
        PlayerManager.instance.playerAnimationManager.PlayActionAnimation("Kick", true, true, true, false, false, true, 0.3f);
    }
    public override void OnStateUpdate()
    {
        if (stateMachine.canAttack && PlayerInputManager.instance.attackPressed && !PlayerManager.instance.isPerformingAction)
        {

            stateMachine.SwitchState(combatFactory.SpinKick());
        }
    }
    public override void OnStateExit()
    {

    }

}
