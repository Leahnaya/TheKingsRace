using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dOffenseKickState : dOffenseBaseState
{

    float legRotation = 0; // angle for leg rotation
    bool kicked = false; // whether they have kicked or not


    public override void EnterState(dOffenseStateManager oSM, dOffenseBaseState previousState){

        oSM.leg.SetActive(true); // activate leg
        kicked = false; // haven't kicked

        oSM.animator.SetBool("IsKickingIdle", true);
        //start kicking routine
        oSM.StartCoroutine(kicking(1f));
    }

    public override void ExitState(dOffenseStateManager oSM, dOffenseBaseState nextState){
        legRotation = 0; // reset leg angle
        oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z); // rotate leg
        oSM.leg.SetActive(false); // reset leg angle
        oSM.animator.SetBool("IsKickingIdle", false);
    }

    public override void UpdateState(dOffenseStateManager oSM){

        //if incapacitated then cooldown
        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState)){
            oSM.SwitchState(oSM.CooldownState);
        }

        //if kicked then cooldown
        if(kicked){
            oSM.SwitchState(oSM.CooldownState);
        }
    }

    public override void FixedUpdateState(dOffenseStateManager oSM){

        //if leg isn't fully extended rotate it
        if(legRotation > -90){
            oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z);
            legRotation -= 20;
        }
        else{
            legRotation = -90;
        }

        //jitter player upwards because of a weird issue with collider
        //oSM.moveController.Move(new Vector3(0,.002f,0));
    }

    //kicking timer
    private IEnumerator kicking(float waitTime){
        yield return new WaitForSeconds(waitTime);
        kicked = true;
    }
}
