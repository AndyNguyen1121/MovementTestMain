using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatIdle : PlayerBaseState
{
    public CombatIdle(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine) : base(combatFactory, stateMachine) { }
    public override void OnStateEnter()
    {
        
    }
    public override void OnStateUpdate()
    {
        if (stateMachine.canAttack && PlayerInputManager.instance.attackPressed && !PlayerManager.instance.isPerformingAction)
        {
            Debug.Log("AttackPressed");
            stateMachine.SwitchState(combatFactory.FistAttack1());
        }
    }
    public override void OnStateExit()
    {

    }

}
