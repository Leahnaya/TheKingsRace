using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialGrappleAirState : dAerialBaseState
{

    float distanceBeneathHook = -10f; // Distance Beneath Hook
    float distanceAfterHook = 8f; // Distance After Hook

    Vector3 initialForceDirection; // Initial Force Direction
    Vector3 desiredPosition; // Desired Player Position
    Vector3 tempForceDir; // Temporary Force Direction for calculating if the position has been reached
    Vector3 updatedYHookPoint; // updated y position of the hook


    bool pointReached; // Whether the player has reached the desired position
    bool handReached = false; // Whether the grapple hand has reached

    float initialForcePower; // How much force to apply initially


    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){
        
        //Reset these Variables
        pointReached = false;
        handReached = false;
        initialForcePower = 120;
        aSM.currentForcePower = initialForcePower;

        //refresh jump number
        aSM.curJumpNum = 0;
        aSM.release = false;

        //Calculate the update y position for the hook
        updatedYHookPoint = new Vector3(aSM.hookPoint.transform.position.x, aSM.hookPoint.transform.position.y - distanceBeneathHook, aSM.hookPoint.transform.position.z);
        
    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){

        //Reset Hand Rotation
        aSM.stickyHandParent.transform.localEulerAngles = Vector3.zero;

        //Deactivate Visuals
        aSM.handController.enabled = false;
        aSM.stickyHandParent.SetActive(false);
        aSM.lr.enabled = false;

        //if the next state is grounded or wallrun
        if(nextState == aSM.GroundedState || nextState == aSM.WallRunState){

            //don't apply the release force
            aSM.release = false;
        }
        else{

            //apply release force
            aSM.release = true;

            //give the player a slight upwards direction on releasing
            aSM.pStats.GravVel = 10;
        }
    }

    public override void UpdateState(dAerialStateManager aSM){

        //if the player has reached the calculated point
        if(pointReached){
            //if grounded
            if(aSM.isGrounded){

                //switch to grounded state
                aSM.SwitchState(aSM.GroundedState);
            }

            //if isn't grounded
            else if(!aSM.isGrounded){

                //switch to falling state
                aSM.SwitchState(aSM.FallingState);
            }

            //if wallrunning
            else if(aSM.isWallRunning){

                //switch to wallrun state
                aSM.SwitchState(aSM.WallRunState);
            }
        }


    }

    public override void FixedUpdateState(dAerialStateManager aSM){

        //If the grapple hand hasn't reached
        if(!handReached){

            //Throw player hand
            ThrowStickyHand(aSM);

            //Apply Regular gravity
            aSM.GravityCalculation(aSM.pStats.PlayerGrav);
        }

        //If the grapple hand has reached and the player hasn't reached the correct point
        else if(!pointReached && handReached){

            //Start Retrieving the grapple hand
            RetrieveStickyHand(aSM);

            //Move the player using the calculated direction and power
            aSM.moveController.Move(initialForceDirection * initialForcePower * Time.deltaTime);

            //Disable gravity while moving
            aSM.GravityCalculation(0);

            //Set Gravity to 0
            aSM.pStats.GravVel = 0;

            //Calculate A temporary force direction for checking if point is reached
            tempForceDir = desiredPosition - aSM.transform.position;
            tempForceDir = tempForceDir.normalized;
            tempForceDir = new Vector3(tempForceDir.x,0,tempForceDir.z).normalized;

            //if the player has approximately reached the right position
            if((aSM.postForceDirection - tempForceDir).magnitude >= .1f && !pointReached){

                //Player has reached the calculated position
                pointReached = true;
            }
        }

        else{

            //normal gravity if grapple is finished
            aSM.GravityCalculation(aSM.pStats.PlayerGrav); 
        }

        //Offset parent rotation for the hand
        if(aSM.stickyHandParent.active){
            aSM.stickyHandParent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, aSM.stickyHandParent.transform.parent.rotation.z * -1.0f);
        }

    }
    
    //Throw Hand toward hookpoint & calculate desired position
    public void ThrowStickyHand(dAerialStateManager aSM){

        //If the sticky hand isn't active
        if(!aSM.stickyHandParent.active){

            //enable hand
            aSM.stickyHandParent.SetActive(true);

            //enable line renderer
            aSM.lr.enabled = true;

            //move hand position to hand
            aSM.stickyHandParent.transform.position = aSM.handPosition.position;

            //enable the character controller on the hand
            aSM.handController.enabled = true;
        }

        //set positioning for the line renderer
        aSM.lr.SetPosition(0,aSM.stickyHandParent.transform.position);
        aSM.lr.SetPosition(1,aSM.handPosition.transform.position);

        //Hand Throw Direction
        Vector3 throwDir = (aSM.hookPoint.transform.position -  aSM.stickyHandParent.transform.position).normalized;

        //move the hand
        aSM.stickyHandParent.GetComponent<CharacterController>().Move(throwDir * 120 * Time.deltaTime);

        //if the hand approximately reaches the correct point
        if(Vector3.Distance(aSM.hookPoint.transform.position, aSM.stickyHandParent.transform.position) <= 3f){

            //Calculate the desired position using current position when hook reaches
            Vector3 updatedXHookPointDirection = (new Vector3(aSM.transform.position.x, 0, aSM.transform.position.z) - new Vector3(aSM.hookPoint.transform.position.x, 0, aSM.hookPoint.transform.position.z)).normalized;
            desiredPosition =  updatedYHookPoint + (updatedXHookPointDirection * -distanceAfterHook);
            initialForceDirection = desiredPosition - aSM.transform.position;
            initialForceDirection = initialForceDirection.normalized;
            aSM.postForceDirection = new  Vector3(initialForceDirection.x, 0, initialForceDirection.z).normalized;

            //Hand has Reached
            handReached = true;
        }
    }

    //Retrieve Grapple Hand
    public void RetrieveStickyHand(dAerialStateManager aSM){
        aSM.lr.SetPosition(0,aSM.stickyHandParent.transform.position);
        aSM.lr.SetPosition(1,aSM.handPosition.transform.position);

        Vector3 catchDir = (aSM.handPosition.transform.position -  aSM.stickyHandParent.transform.position).normalized;
        if(Vector3.Distance(aSM.handPosition.transform.position, aSM.stickyHandParent.transform.position) > .5f){
            aSM.stickyHandParent.GetComponent<CharacterController>().Move(catchDir * 30 * Time.deltaTime);
        }
    }


    
}
