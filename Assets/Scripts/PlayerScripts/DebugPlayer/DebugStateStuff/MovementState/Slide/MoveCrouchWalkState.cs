using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCrouchWalkState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Crouch Walk State");
    }
    
    public override void UpdateState(MoveStateManager mSM){

    }

    public override void FixedUpdateState(MoveStateManager mSM){

    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
