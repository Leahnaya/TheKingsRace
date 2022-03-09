using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveRecoveringState : dMoveBaseState
{
    ////// ADD SOMETHING THAT CHECKS ANIMATION FINISH BEFORE GO TO IDLE

    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        mSM.pStats.GravVel = 80; // resets gravVel
        mSM.capCol.enabled = false; // disable capsule collider
        mSM.moveController.enabled = true; // enable move controller
        mSM.rB.isKinematic = true; // enable kinematic
        mSM.rB.detectCollisions = false; // detect collisions false
        mSM.CancelMomentum(); // reset player variables
        mSM.moveController.Move(new Vector3(0,2,0));
    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }

    public override void UpdateState(dMoveStateManager mSM){
        
        //swap to idle state
        mSM.SwitchState(mSM.IdleState);
    }

    public override void FixedUpdateState(dMoveStateManager mSM){

    }
}
