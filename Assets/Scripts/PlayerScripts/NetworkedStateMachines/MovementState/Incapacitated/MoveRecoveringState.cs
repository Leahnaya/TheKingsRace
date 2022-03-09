using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRecoveringState : MoveBaseState
{
    ////// ADD SOMETHING THAT CHECKS ANIMATION FINISH BEFORE GO TO IDLE

    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        mSM.pStats.GravVel = 50; // resets gravVel
        mSM.capCol.enabled = false; // disable capsule collider
        mSM.moveController.enabled = true; // enable move controller
        mSM.rB.isKinematic = true; // enable kinematic
        mSM.CancelMomentum(); // reset player variables
        mSM.moveController.Move(new Vector3(0,2,0));
    }

    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }

    public override void UpdateState(MoveStateManager mSM){
        
        //swap to idle state
        mSM.SwitchState(mSM.IdleState);
    }

    public override void FixedUpdateState(MoveStateManager mSM){

    }
}
