using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialGrappleAirState : dAerialBaseState
{

    Vector3 initialForceDirection;

    float distanceBeneathHook = 4f;
    float distanceAfterHook = 5f;
    Vector3 desiredPosition;

    bool pointReached = false;


    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){

        //refresh jump number
        aSM.curJumpNum = 0;

        Vector3 updatedYHookPoint = new Vector3(aSM.hookPoint.transform.position.x, aSM.hookPoint.transform.position.y - distanceBeneathHook, aSM.hookPoint.transform.position.z);
        Vector3 updatedXHookPointDirection = (new Vector3(aSM.transform.position.x, 0, aSM.transform.position.z) - new Vector3(aSM.hookPoint.transform.position.x, 0, aSM.hookPoint.transform.position.z)).normalized;

        desiredPosition =  updatedYHookPoint + (updatedXHookPointDirection * -distanceAfterHook);
        //rope length limit
        initialForceDirection = desiredPosition - aSM.transform.position;
        initialForceDirection = initialForceDirection.normalized;



    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){

    }

    public override void UpdateState(dAerialStateManager aSM){

    }

    public override void FixedUpdateState(dAerialStateManager aSM){
        
        ////////ADD A LINE RENDERER WHEN WE GET THE HAND MODEL
        //Draw Line between player and hookpoint for debug purposes
        Debug.DrawRay(aSM.transform.position, initialForceDirection); //Visual of line

        //Apply default gravity
        if(!pointReached){
            aSM.moveController.Move(initialForceDirection * .1f);
            aSM.GravityCalculation(.1f); 
        }
        else{
            aSM.GravityCalculation(aSM.pStats.PlayerGrav); 
        }
    }

    float CalculateInitialForcePower(){
        return 0;
    }

    float ForceDissipation(){
        return 0;
    }


}
