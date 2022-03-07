using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRagdollState : MoveBaseState
{

    float ragTime; // ragdoll timer
    bool beginRagTimer = false; // whether ragtimer has started

    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){

        ragTime = mSM.pStats.RecovTime; // how long to be ragdolled
        mSM.prevRot = mSM.transform.localEulerAngles; // save previous rotation
        mSM.capCol.enabled = true; // enable capsule collider
        mSM.moveController.enabled = false; // disable move controller
        mSM.rB.isKinematic = false; // disable kinematic
        mSM.rB.detectCollisions = true; // detect collisions

        //apply force
        mSM.rB.AddForce(mSM.dirHit, ForceMode.Impulse);
    }

    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }

    public override void UpdateState(MoveStateManager mSM){

        //if player hasn't touched the ground don't start timer
        if(!beginRagTimer){
            beginRagTimer = Physics.Raycast(mSM.transform.position, -Vector3.up, mSM.distToGround + 1f);
        }

        //start timer
        else{
            ragTime -= Time.deltaTime;
        }

    }

    public override void FixedUpdateState(MoveStateManager mSM){

        //if ragtimer is over then recover
        if(ragTime <= 0 && beginRagTimer){
            ragTime = 0;
            beginRagTimer = false;

            mSM.SwitchState(mSM.RecoveringState);
        }
    }
}
