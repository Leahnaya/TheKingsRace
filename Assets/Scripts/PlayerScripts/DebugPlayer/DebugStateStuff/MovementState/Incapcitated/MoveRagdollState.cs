using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRagdollState : MoveBaseState
{
    float ragTime;
    Vector3 prevRot;
    bool beginRagTimer = false;

    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Ragdoll State");

        ragTime = mSM.pStats.RecovTime;
        prevRot = mSM.transform.localEulerAngles;
        mSM.capCol.enabled = true;
        mSM.moveController.enabled = false;
        mSM.rB.isKinematic = false;
        mSM.rB.detectCollisions = true;

        mSM.rB.AddForce(mSM.dirHit, ForceMode.Impulse);
    }

    public override void UpdateState(MoveStateManager mSM){
        if(!beginRagTimer){
            beginRagTimer = Physics.Raycast(mSM.transform.position, -Vector3.up, mSM.distToGround + 1f);
        }
        else{
            ragTime -= Time.deltaTime;
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        //Has to be in Fixed Update because it has player movement
        if(ragTime <= 0 && beginRagTimer){
            ragTime = 0;
            beginRagTimer = false;

            mSM.pStats.GravVel = 50;
            mSM.capCol.enabled = false;
            mSM.moveController.enabled = true;
            mSM.rB.isKinematic = true;
            mSM.rB.detectCollisions = false;
            mSM.transform.localEulerAngles = prevRot;
            mSM.SwitchState(mSM.RecoveringState);
        }
    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
