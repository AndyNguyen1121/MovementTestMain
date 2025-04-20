using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStateMachine : MonoBehaviour
{
    public CombatStateFactory combatFactory;

    public PlayerBaseState currentState;

    private void Start()
    {
        combatFactory = new CombatStateFactory(this);
        currentState = combatFactory.CombatIdle();
        currentState.OnStateEnter();
    }
    private void Update()
    {
        currentState.OnStateUpdate();
    }
    public void SwitchState(PlayerBaseState newState)
    {
        currentState.OnStateExit();
        currentState = newState;
    }
}
