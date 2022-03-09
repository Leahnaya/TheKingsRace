using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialGrappleAirState : dAerialBaseState
{

    bool spaceHeld = true;
    float ropeLength; // current rope length
    float inclinationAngle; // inclination angle
    float theta = -1; // theta for rope angle
    Vector3 hookPointRight; // right vector of the hook point
    Vector3 curXZDir; // current straight line between player and hook point ignoring y axis
    Vector3 oldXZDir; // old straight line between player and hook point ignoring y axis

    Vector3 swingDirection; // Swing direction
    float swingSpeed = 10; // cur swing speed
    Vector3 tensionDirection; // tension direction
    float tensionForce; // tension force amplifier
    
    float swingMom; // swing Mom amplifier
    float oldSwingMom; // old swing Momentum amplifier
    Vector3 momDirection; // momentum direction
    Vector3 tensionMomDirection; // tension momentum direction
    
    float defaultGraceTimer = 1.0f; // default timer
    float graceTimer; // grace period at the end of the swing so the player has time to let go 
    bool swingGrace = true; // is the grace timer over

    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){

        //refresh jump number
        aSM.curJumpNum = 0;

        //rope length limit
        ropeLength = Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position);
        if(ropeLength > aSM.maxGrappleDistance){
            ropeLength = aSM.maxGrappleDistance;
        }

        //old and cur direction vector for player to hookpoint
        oldXZDir = (new Vector3(aSM.hookPoint.transform.position.x,0,aSM.hookPoint.transform.position.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;
        curXZDir = (new Vector3(aSM.hookPoint.transform.position.x,0,aSM.hookPoint.transform.position.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;

        //swing momentum calculation
        swingMom = CalculateSwingMom(aSM.mSM.driftVel.magnitude * 50f, aSM);
        oldSwingMom = swingMom;

        //Initialize variables
        aSM.pStats.GravVel = -1; // grav vel is adjusted so things work
        aSM.release = false; // player hasn't released
        aSM.lerpRelease = Vector3.zero; // reset lerp release
        spaceHeld = true;
        graceTimer = defaultGraceTimer; //sets grace timer 
    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){

        //If not going into grapple grounded state
        if(nextState != aSM.GrappleGroundedState){
            aSM.release = true;
            aSM.pStats.GravVel = 0;
            aSM.forceDirection = Vector3.zero;
        }

        //If going into grapple grounded state
        else{
            aSM.pStats.GravVel = 0;
            aSM.forceDirection = Vector3.zero;
        }
    }

    public override void UpdateState(dAerialStateManager aSM){
        
        //if pressing E or ragdolling then falling
        if(((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !aSM.eHeld && !aSM.pStats.IsPaused) || (aSM.mSM.currentState == aSM.mSM.RagdollState)){
            aSM.SwitchState(aSM.FallingState);
        }
        else if((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton2)) && aSM.eHeld){
            aSM.eHeld = false;
        }

        //if pressing Jump then jump
        else if(Input.GetButton("Jump") && !spaceHeld && !aSM.pStats.IsPaused){
            aSM.SwitchState(aSM.JumpingState);
        }
        else if(!(Input.GetButton("Jump")) && spaceHeld){
            spaceHeld = false;
        }

        //if grounded then grapple grounded
        else if(aSM.isGrounded){
            aSM.SwitchState(aSM.GrappleGroundedState);
        }

        //if wallrunning then wallrun
        else if(aSM.isWallRunning){
            aSM.SwitchState(aSM.WallRunState);
        }

    }

    public override void FixedUpdateState(dAerialStateManager aSM){
        
        ////////ADD A LINE RENDERER WHEN WE GET THE HAND MODEL
        //Draw Line between player and hookpoint for debug purposes
        Debug.DrawRay(aSM.transform.position, (aSM.hookPoint.transform.position - aSM.transform.position)); //Visual of line

        //Calculate tether force direction based on hookpoint
        if (Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position) >= ropeLength )
        {
            aSM.forceDirection = CalculateForceDirection(1, aSM.pStats.GravVel, aSM.hookPoint.transform.position, aSM) + RopeLengthOffset(aSM.hookPoint.transform.position, Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position), aSM);
        }
        else{
            aSM.forceDirection = Vector3.zero;
        }

        //Move player based on their inputs
        aSM.moveController.Move(SwingMoveController(aSM));

        //if Swing Momentum isn't zero then move player
        if(swingMom != 0){
            aSM.moveController.Move(CalculateMomentumDirection(aSM.pStats.GravVel, aSM.hookPoint.transform.position, aSM));
        
            swingMom -= .5f;
        }
        if(swingMom<0) swingMom = 0;

        //Calculate temp release at every position
        aSM.tempRelease = CalculateSwingReleaseForce();

        //Apply default gravity
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

    }

    //Calculate the tether direction vector and how much force that vector needs
    Vector3 CalculateForceDirection(float mass, float g, Vector3 hPoint, dAerialStateManager aSM){

        //tension direction and angle calculation
        tensionDirection = (hPoint - aSM.transform.position).normalized;
        inclinationAngle = Vector3.Angle((aSM.transform.position - hPoint).normalized, -aSM.transform.up);
        theta = Mathf.Deg2Rad * inclinationAngle;
        if(theta<=.1) theta = 0;

        //How much force the tension needs
        tensionForce = mass * -g * Mathf.Cos(theta);

        //force direction calculation based on tension direction and force
        Vector3 fDirection = tensionDirection * tensionForce;

        //return force direction
        return fDirection;

    }

    Vector3 CalculateMomentumDirection(float g, Vector3 hPoint, dAerialStateManager aSM){

        tensionMomDirection = (hPoint - aSM.transform.position).normalized;
        hookPointRight = Vector3.Cross(oldXZDir, aSM.transform.up).normalized;
        momDirection = -1 * Vector3.Cross(hookPointRight, tensionMomDirection).normalized;

        //if player is on the other side of hookpoint and swingMom is lower then update oldXZDir
        if(oldXZDir != curXZDir && swingMom <= (oldSwingMom*(.75f))){
            if(graceTimer >= 0){
                swingGrace = false;
                graceTimer -= .1f;
            }
            else{
                oldSwingMom = swingMom;
                oldXZDir = (new Vector3(aSM.hookPoint.transform.position.x,0,aSM.hookPoint.transform.position.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;   
                swingGrace = true;
                graceTimer = defaultGraceTimer;
            }
            
        }

        //current line between hookpoint and player
        curXZDir = (new Vector3(hPoint.x,0,hPoint.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;
        Debug.DrawRay(aSM.transform.position,  momDirection * Time.deltaTime* 100, Color.green);

        //return momentum dir * mom force
        return (momDirection * Time.deltaTime * swingMom);

    }

    //Calculates the players initial swing momentum using their height and their current velocity
    float CalculateSwingMom(float playerSpeed, dAerialStateManager aSM){

        //Calculate the players height compared to the lowest point in the swing
        float swingHeight = aSM.transform.position.y - (aSM.hookPoint.transform.position.y - ropeLength);
        if(swingHeight <= 1){
            swingHeight = 1;
        }

        //calculates swing momentum based on height ands speed
        float sMom = playerSpeed + (swingHeight * 3f);
        if(sMom > aSM.maxSwingMom){
            sMom = aSM.maxSwingMom;
        }

        //returns swing momentum
        return sMom;

    }
    
    //Special movement for the player while they swing
    Vector3 SwingMoveController(dAerialStateManager aSM){

        //WASD input
        float inputVert = Input.GetAxis("Vertical");
        float inputHor = Input.GetAxis("Horizontal");

        //input is zero when nothing is pressed to prevent button easing values
        if((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))) inputVert = 0;
        if((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) inputHor = 0;

        //Swingspeed build up
        if((inputVert != 0 || inputHor != 0) && swingSpeed < aSM.maxSwingSpeed){
            swingSpeed += aSM.swingAcc;
        }
        else if((inputVert != 0 || inputHor != 0) && swingSpeed >= aSM.maxSwingSpeed){
            swingSpeed = aSM.maxSwingSpeed;
        }
        else if((inputVert == 0 && inputHor == 0)){
            swingSpeed = aSM.minSwingSpeed;
        }

        //Swing direction based on player input
        swingDirection = Vector3.Cross(tensionDirection, ((aSM.transform.right * -inputVert) + (aSM.transform.forward * inputHor))).normalized;

        //Swing movement with swing speed added
        Vector3 swingMovement = (swingDirection * Time.deltaTime * swingSpeed);
        
        //returns swing movement vector
        return (swingMovement);

    }

    Vector3 RopeLengthOffset(Vector3 hPoint, float curDistance, dAerialStateManager aSM){
        
        //How powerful our offset movement has to be
        float offsetPower = ((curDistance - ropeLength) * 200f);

        //The direction we need to apply force to offset when the rope gets lengthened beyond the necessary point
        Vector3 tenDirOffset = (hPoint - aSM.transform.position).normalized;
 
        //returns rope offset direction * power
        return tenDirOffset * offsetPower * Time.deltaTime;

    }

    Vector3 CalculateSwingReleaseForce(){
        
        //Swing release direction
        Vector3 releaseSwingForceDirection = momDirection * ((swingMom) + 10);
        releaseSwingForceDirection = new Vector3(releaseSwingForceDirection.x,0,releaseSwingForceDirection.z);

        //if swingMom is low there is no release force
        if(swingMom < 2){
            return Vector3.zero;
        }

        //return swing release direction
        return releaseSwingForceDirection * Time.deltaTime;

    }
}
