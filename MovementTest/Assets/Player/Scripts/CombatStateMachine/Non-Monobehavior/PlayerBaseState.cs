using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState 
{
    protected CombatStateFactory combatFactory;
    protected PlayerCombatStateMachine stateMachine;

    public PlayerBaseState(CombatStateFactory combatFactory, PlayerCombatStateMachine stateMachine)
    {
        this.combatFactory = combatFactory;
        this.stateMachine = stateMachine;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();

}
