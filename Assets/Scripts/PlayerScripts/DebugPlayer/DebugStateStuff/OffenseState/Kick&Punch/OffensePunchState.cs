using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensePunchState : OffenseBaseState
{

    float legRotation = 0;
    bool kicked = false;


    public override void EnterState(OffenseStateManager oSM, OffenseBaseState previousState){
        oSM.leg.SetActive(true);
        kicked = false;

        oSM.StartCoroutine(kicking(1f));
    }

    public override void ExitState(OffenseStateManager oSM, OffenseBaseState nextState){
        legRotation = 0;
        oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z);
        oSM.leg.SetActive(false);
    }

    public override void UpdateState(OffenseStateManager oSM){
        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState || oSM.aSM.currentState == oSM.aSM.GrappleGroundedState)){
            oSM.SwitchState(oSM.CooldownState);
        }

        if(kicked){
            oSM.SwitchState(oSM.CooldownState);
        }
    }

    public override void FixedUpdateState(OffenseStateManager oSM){
        if(legRotation > -90){
            oSM.leg.transform.eulerAngles = new Vector3(legRotation, oSM.leg.transform.eulerAngles.y, oSM.leg.transform.eulerAngles.z);
            legRotation -= 20;
        }
        else{
            legRotation = -90;
        }
        oSM.moveController.Move(new Vector3(0,.002f,0));
    }

    private IEnumerator kicking(float waitTime){
        yield return new WaitForSeconds(waitTime);
        kicked = true;
    }
}
