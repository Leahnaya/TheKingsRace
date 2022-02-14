using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGlidingState : AerialBaseState
{
    float tempTraction;

    public override void EnterState(AerialStateManager aSM){
        Debug.Log("Gliding State");

        tempTraction = aSM.pStats.Traction;
        aSM.pStats.Traction = 1.0f;
    }

    public override void UpdateState(AerialStateManager aSM){
        if(!Input.GetButton("Jump")){
            aSM.pStats.Traction = tempTraction;
            aSM.SwitchState(aSM.FallingState);
        }

        if(aSM.isGrounded){
            aSM.pStats.Traction = tempTraction;
            aSM.SwitchState(aSM.GroundedState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(9);
    }

    public override void OnCollisionEnter(AerialStateManager aSM){

    }
}
