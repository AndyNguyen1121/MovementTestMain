using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateFactory
{
    public PlayerCombatStateMachine combatStateMachine;

    private FistAttack1 fistAttack1;
    private FistAttack2 fistAttack2;
    private FistAttack3 fistAttack3;   
    private Kick1 kick1;
    private SpinKick spinKick;

    private CombatIdle combatIdle;
    public CombatStateFactory(PlayerCombatStateMachine combatStateMachine)
    {
        this.combatStateMachine = combatStateMachine;

        combatIdle = new CombatIdle(this, combatStateMachine);
        fistAttack1 = new FistAttack1(this, combatStateMachine);
        fistAttack2 = new FistAttack2(this, combatStateMachine);
        fistAttack3 = new FistAttack3(this, combatStateMachine);
        kick1 = new Kick1(this, combatStateMachine);
        spinKick = new SpinKick(this, combatStateMachine);
        
    }
    public PlayerBaseState CombatIdle()
    {
        return combatIdle;
    }
    public PlayerBaseState FistAttack1()
    {
        return fistAttack1;
    }

    public PlayerBaseState FistAttack2()
    {
        return fistAttack2;
    }

    public PlayerBaseState FistAttack3()
    {
        return fistAttack3;
    }

    public PlayerBaseState Kick1()
    {
        return kick1;   
    }

    public PlayerBaseState SpinKick()
    {
        return spinKick;
    }
   
}
