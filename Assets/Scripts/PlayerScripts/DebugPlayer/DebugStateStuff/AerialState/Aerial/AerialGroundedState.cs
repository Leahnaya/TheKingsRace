using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGroundedState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM){
        Debug.Log("Grounded State");
    }

    public override void UpdateState(AerialStateManager aSM){
        if(aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.FallingState);
        }
        else if(aSM.pStats.GravVel > 0){
            aSM.SwitchState(aSM.JumpingState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }

    public override void OnCollisionEnter(AerialStateManager aSM){

    }
}
