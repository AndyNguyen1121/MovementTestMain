using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatIdle : PlayerBaseState
{
    public CombatIdle(CombatStateFactory combatFactory, PlayerCombatStateMachine combatStateMachine) : base(combatFactory, combatStateMachine) { }
    public override void OnStateEnter()
    {
        Debug.Log("Hello from idle state.");
    }
    public override void OnStateUpdate()
    {

    }
    public override void OnStateExit()
    {

    }

}
