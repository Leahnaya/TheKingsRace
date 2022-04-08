using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGrappleAirState : AerialBaseState
{

    Vector3 initialForceDirection;

    float distanceBeneathHook = -3f;
    float distanceAfterHook = 8f;
    Vector3 desiredPosition;

    bool pointReached = false;

    float initialForcePower = 100;


    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        

        distanceBeneathHook = -3f;
        distanceAfterHook = 8f;
        pointReached = false;
        initialForcePower = 100;
        
        //refresh jump number
        aSM.curJumpNum = 0;
        aSM.release = false;

        Vector3 updatedYHookPoint = new Vector3(aSM.hookPoint.transform.position.x, aSM.hookPoint.transform.position.y - distanceBeneathHook, aSM.hookPoint.transform.position.z);
        Vector3 updatedXHookPointDirection = (new Vector3(aSM.transform.position.x, 0, aSM.transform.position.z) - new Vector3(aSM.hookPoint.transform.position.x, 0, aSM.hookPoint.transform.position.z)).normalized;

        desiredPosition =  updatedYHookPoint + (updatedXHookPointDirection * -distanceAfterHook);
        //rope length limit
        initialForceDirection = desiredPosition - aSM.transform.position;
        initialForceDirection = initialForceDirection.normalized;

        aSM.postForceDirection = new  Vector3(initialForceDirection.x, 0, initialForceDirection.z).normalized;
        aSM.currentForcePower = initialForcePower;

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){
        if(nextState == aSM.GroundedState || nextState == aSM.WallRunState){
            aSM.release = false;
        }
        else{
            aSM.release = true;
            aSM.pStats.GravVel = 10;
        }
    }

    public override void UpdateState(AerialStateManager aSM){
        if(pointReached){
            //if grounded at when the point is reached then goto grounded state
            if(aSM.isGrounded){
                aSM.SwitchState(aSM.GroundedState);
            }
            else if(!aSM.isGrounded){
                aSM.SwitchState(aSM.FallingState);
            }
            //if wallrunning then wallrun
            else if(aSM.isWallRunning){
                aSM.SwitchState(aSM.WallRunState);
            }
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        
        ////////ADD A LINE RENDERER WHEN WE GET THE HAND MODEL
        //Draw Line between player and hookpoint for debug purposes
        Debug.DrawRay(aSM.transform.position, initialForceDirection); //Visual of line

        Vector3 tempForceDir = desiredPosition - aSM.transform.position;
        tempForceDir = tempForceDir.normalized;
        tempForceDir = new Vector3(tempForceDir.x,0,tempForceDir.z).normalized;

        if((aSM.postForceDirection - tempForceDir).magnitude >= .1f && !pointReached){
            pointReached = true;
        }

        //Apply default gravity
        if(!pointReached){
            aSM.moveController.Move(initialForceDirection * initialForcePower * Time.deltaTime);
            aSM.GravityCalculation(0);
            aSM.pStats.GravVel = 0;
        }
        else{
            //currentForcePower;
            aSM.GravityCalculation(aSM.pStats.PlayerGrav); 
        }
    }


    
}
