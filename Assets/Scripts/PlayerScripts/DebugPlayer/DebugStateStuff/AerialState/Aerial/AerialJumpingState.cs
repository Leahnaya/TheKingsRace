using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialJumpingState : AerialBaseState
{

    public override void EnterState(AerialStateManager aSM){
        Debug.Log("Jumping State");
    }

    public override void UpdateState(AerialStateManager aSM){

        if(aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.FallingState);
        }

        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }

    public override void OnCollisionEnter(AerialStateManager aSM){

    }
}
