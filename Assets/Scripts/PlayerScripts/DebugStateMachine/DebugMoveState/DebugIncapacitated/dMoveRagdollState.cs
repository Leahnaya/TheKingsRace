using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveRagdollState : dMoveBaseState
{

    float ragTime; // ragdoll timer
    Vector3 prevRot; // previous rotation before ragdolled
    bool beginRagTimer = false; // whether ragtimer has started

    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

        ragTime = mSM.pStats.RecovTime; // how long to be ragdolled
        mSM.prevRot = mSM.transform.localEulerAngles; // save previous rotation
        mSM.capCol.enabled = true; // enable capsule collider
        mSM.moveController.enabled = false; // disable move controller
        mSM.rB.isKinematic = false; // disable kinematic
        mSM.rB.detectCollisions = true; // detect collisions

        //apply force
        mSM.rB.AddForce(mSM.dirHit, ForceMode.Impulse);
    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }

    public override void UpdateState(dMoveStateManager mSM){

        //if player hasn't touched the ground don't start timer
        if(!beginRagTimer){
            beginRagTimer = Physics.Raycast(mSM.transform.position, -Vector3.up, mSM.distToGround + 1f);
        }

        //start timer
        else{
            ragTime -= Time.deltaTime;
        }

    }

    public override void FixedUpdateState(dMoveStateManager mSM){

        //if ragtimer is over then recover
        if(ragTime <= 0 && beginRagTimer){
            ragTime = 0;
            beginRagTimer = false;

            mSM.SwitchState(mSM.RecoveringState);
        }
    }
}
