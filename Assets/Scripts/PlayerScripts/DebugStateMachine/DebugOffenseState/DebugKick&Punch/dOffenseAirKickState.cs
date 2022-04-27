using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dOffenseAirKickState : dOffenseBaseState
{

    private float legRotation = 0; // angle for leg rotation
    bool kicked = false; // whether they have kicked or not

    public override void EnterState(dOffenseStateManager oSM, dOffenseBaseState previousState){

        oSM.leg.SetActive(true); // activate leg
        legRotation = -90;
        oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z); // rotate leg
        kicked = false; // haven't kicked

        //start kicking routine
        oSM.StartCoroutine(kicking(8f));
        oSM.GetComponent<Animator>().SetBool("isAerialKick", true);
    }

    public override void ExitState(dOffenseStateManager oSM, dOffenseBaseState nextState){
        legRotation = 0; // reset leg angle
        oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z); // rotate leg
        oSM.leg.SetActive(false); // reset leg angle
        oSM.GetComponent<Animator>().SetBool("isAerialKick", false);
    }

    public override void UpdateState(dOffenseStateManager oSM){
        
        //if incapacitated then cooldown
        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState)){
            oSM.SwitchState(oSM.CooldownState);
        }

        //if kicked or grounded then cooldown
        if(kicked || (oSM.aSM.currentState == oSM.aSM.GroundedState)){
            oSM.SwitchState(oSM.CooldownState);
        }
    }

    public override void FixedUpdateState(dOffenseStateManager oSM){

        //jitter player upwards because of a weird issue with collider
        oSM.moveController.Move(new Vector3(0,.002f,0));
    }

    //kicking timer
    private IEnumerator kicking(float waitTime){
        yield return new WaitForSeconds(waitTime);
        kicked = true;
    }
}
