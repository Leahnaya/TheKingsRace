using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialGrappleAirState : dAerialBaseState
{

    Vector3 initialForceDirection;

    float distanceBeneathHook;
    float distanceAfterHook;
    Vector3 desiredPosition;
    Vector3 tempForceDir;
    Vector3 updatedYHookPoint;


    bool pointReached;
    bool handReached = false;

    float initialForcePower;


    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){
        

        distanceBeneathHook = -10f;
        distanceAfterHook = 8f;
        pointReached = false;
        initialForcePower = 120;
        
        //refresh jump number
        aSM.curJumpNum = 0;
        aSM.release = false;

        updatedYHookPoint = new Vector3(aSM.hookPoint.transform.position.x, aSM.hookPoint.transform.position.y - distanceBeneathHook, aSM.hookPoint.transform.position.z);
        aSM.currentForcePower = initialForcePower;
        handReached = false;

    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){
        aSM.stickyHandParent.transform.localEulerAngles = Vector3.zero;
        aSM.handController.enabled = false;
        aSM.stickyHandParent.SetActive(false);
        aSM.lr.enabled = false;

        if(nextState == aSM.GroundedState || nextState == aSM.WallRunState){
            aSM.release = false;
        }
        else{
            aSM.release = true;
            aSM.pStats.GravVel = 10;
        }
    }

    public override void UpdateState(dAerialStateManager aSM){
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

    public override void FixedUpdateState(dAerialStateManager aSM){

        ////////ADD A LINE RENDERER WHEN WE GET THE HAND MODEL
        //Draw Line between player and hookpoint for debug purposes
        Debug.DrawRay(aSM.transform.position, initialForceDirection); //Visual of line

        //Apply default gravity
        if(!handReached){
            ThrowStickyHand(aSM);
            aSM.GravityCalculation(aSM.pStats.PlayerGrav);
        }
        else if(!pointReached && handReached){
            RetrieveStickyHand(aSM);
            aSM.moveController.Move(initialForceDirection * initialForcePower * Time.deltaTime);
            aSM.GravityCalculation(0);
            aSM.pStats.GravVel = 0;

            tempForceDir = desiredPosition - aSM.transform.position;
            tempForceDir = tempForceDir.normalized;
            tempForceDir = new Vector3(tempForceDir.x,0,tempForceDir.z).normalized;

            if((aSM.postForceDirection - tempForceDir).magnitude >= .1f && !pointReached){
                pointReached = true;
            }
        }
        else{
            //currentForcePower;
            aSM.GravityCalculation(aSM.pStats.PlayerGrav); 
        }

        if(aSM.stickyHandParent.active){
            aSM.stickyHandParent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, aSM.stickyHandParent.transform.parent.rotation.z * -1.0f);
        }

    }

    public void ThrowStickyHand(dAerialStateManager aSM){
        if(!aSM.stickyHandParent.active){
            aSM.stickyHandParent.SetActive(true);
            aSM.lr.enabled = true;
            aSM.stickyHandParent.transform.position = aSM.handPosition.position;
            aSM.handController.enabled = true;
        }

        aSM.lr.SetPosition(0,aSM.stickyHandParent.transform.position);
        aSM.lr.SetPosition(1,aSM.handPosition.transform.position);

        Vector3 throwDir = (aSM.hookPoint.transform.position -  aSM.stickyHandParent.transform.position).normalized;

        aSM.stickyHandParent.GetComponent<CharacterController>().Move(throwDir * 120 * Time.deltaTime);
        if(Vector3.Distance(aSM.hookPoint.transform.position, aSM.stickyHandParent.transform.position) <= 3f){

            Vector3 updatedXHookPointDirection = (new Vector3(aSM.transform.position.x, 0, aSM.transform.position.z) - new Vector3(aSM.hookPoint.transform.position.x, 0, aSM.hookPoint.transform.position.z)).normalized;

            desiredPosition =  updatedYHookPoint + (updatedXHookPointDirection * -distanceAfterHook);
            //rope length limit
            initialForceDirection = desiredPosition - aSM.transform.position;
            initialForceDirection = initialForceDirection.normalized;

            aSM.postForceDirection = new  Vector3(initialForceDirection.x, 0, initialForceDirection.z).normalized;
            handReached = true;
        }
    }

    public void RetrieveStickyHand(dAerialStateManager aSM){
        aSM.lr.SetPosition(0,aSM.stickyHandParent.transform.position);
        aSM.lr.SetPosition(1,aSM.handPosition.transform.position);

        Vector3 catchDir = (aSM.handPosition.transform.position -  aSM.stickyHandParent.transform.position).normalized;
        if(Vector3.Distance(aSM.handPosition.transform.position, aSM.stickyHandParent.transform.position) > .5f){
            aSM.stickyHandParent.GetComponent<CharacterController>().Move(catchDir * 30 * Time.deltaTime);
        }
    }


    
}
