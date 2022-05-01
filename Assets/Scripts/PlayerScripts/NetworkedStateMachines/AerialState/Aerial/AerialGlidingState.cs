using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGlidingState : AerialBaseState
{
    bool rotateGlider = false;

    Vector3 gliderInitialPos = new Vector3(0,.0058f,-.0014f);
    Vector3 gliderInitialRot = new Vector3(80,-180,0);

    Vector3 gliderEndPos = new Vector3(0,.0355f,.0037f);
    Vector3 gliderEndRot = new Vector3(0,-180f,0);

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        //Modify base traction
        aSM.pStats.CurTraction = 1.0f;
        aSM.pStats.GravVel = -3;
        aSM.GetComponent<Animator>().SetBool("isGliding", true);
        rotateGlider = true;

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

        //return traction to normal
        aSM.pStats.CurTraction = aSM.pStats.Traction;
        aSM.GetComponent<Animator>().SetBool("isGliding", false);

        aSM.glider.transform.localPosition = gliderInitialPos;
        aSM.glider.transform.localEulerAngles = gliderInitialRot;
    }

    public override void UpdateState(AerialStateManager aSM){

        if(rotateGlider){
            rotateGlider = RotateGlider(aSM);
        }

        //if not holding jump fall
        if(!Input.GetButton("Jump") || (aSM.mSM.currentState == aSM.mSM.RagdollState)){
            aSM.SwitchState(aSM.FallingState);
        }

        //if is grounded then grounded
        else if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }
        
        //if isWallrunning and in state that allows it wallrun
        else if(aSM.isWallRunning){
            aSM.SwitchState(aSM.WallRunState);
        }

        //if can grapple and in state that allows it grapple
        else if(aSM.CheckGrapple()){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        
        //modified gravity calculation to fall slower
        aSM.GravityCalculation(9);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }

    private bool RotateGlider(AerialStateManager aSM){

        if(aSM.glider.transform.localEulerAngles.x >= 0 && aSM.glider.transform.localEulerAngles.x <= 90){
            if(aSM.glider.transform.localPosition != gliderEndPos){
                aSM.glider.transform.localPosition = Vector3.Lerp(aSM.glider.transform.localPosition, gliderEndPos, 50*Time.deltaTime);
            }
            if(aSM.glider.transform.localEulerAngles != gliderEndRot){
                aSM.glider.transform.localEulerAngles = new Vector3(aSM.glider.transform.localEulerAngles.x - 30,aSM.glider.transform.localEulerAngles.y,aSM.glider.transform.localEulerAngles.z);
            }   
            
            return true;
        }
        else{
            aSM.glider.transform.localEulerAngles = new Vector3(0,aSM.glider.transform.localEulerAngles.y,aSM.glider.transform.localEulerAngles.z);
            return false;
        }
    }
}
