using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateFactory
{
    public PlayerCombatStateMachine combatStateMachine;

    private FistAttack1 fistAttack1;
    private CombatIdle combatIdle;
    public CombatStateFactory(PlayerCombatStateMachine combatStateMachine)
    {
        this.combatStateMachine = combatStateMachine;

        fistAttack1 = new FistAttack1(this, combatStateMachine);
        combatIdle = new CombatIdle(this, combatStateMachine);
    }

    public PlayerBaseState FistAttack1()
    {
        return fistAttack1;
    }

    public PlayerBaseState CombatIdle()
    {
        return combatIdle;
    }
}
