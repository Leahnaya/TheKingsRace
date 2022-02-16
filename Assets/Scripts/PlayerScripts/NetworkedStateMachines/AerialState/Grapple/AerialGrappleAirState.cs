using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGrappleAirState : AerialBaseState
{
    float ropeLength; // current rope length
    Vector3 swingDirection; // Swing direction
    float inclinationAngle; // inclination angle
    float theta = -1; // theta
    float swingMom;
    Vector3 tensionMomDirection;
    Vector3 hookPointRight;
    Vector3 momDirection;
    Vector3 curXZDir;
    Vector3 oldXZDir;
    float swingSpeed = 10;
    bool swingback = false; //swing the player back
    float oldSwingMom;
    Vector3 tensionDirection;
    float tensionForce;

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

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
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

        if(nextState != aSM.GrappleGroundedState){
            aSM.release = true;
            aSM.pStats.GravVel = 0;
            aSM.forceDirection = Vector3.zero;
            swingback = true;
        }
        else{
            aSM.pStats.GravVel = 0;
            aSM.forceDirection = Vector3.zero;
            swingback = true;
        }
    }

    public override void UpdateState(AerialStateManager aSM){

        if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !aSM.eHeld){
            aSM.SwitchState(aSM.FallingState);
        }
        else if((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton2))){
            aSM.eHeld = false;
        }

        if(Input.GetButton("Jump")){
 
            aSM.SwitchState(aSM.JumpingState);
        }

        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GrappleGroundedState);
        }

    }

    public override void FixedUpdateState(AerialStateManager aSM){
            
        Debug.DrawRay(aSM.transform.position, (aSM.hookPoint.transform.position - aSM.transform.position)); //Visual of line

        //Debug.Log(Vector3.Distance(gameObject.transform.position, hookPoint.transform.position));
        //Calculate tether force direction based on hookpoint
        if (Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position) >= ropeLength )
        {
            aSM.forceDirection = CalculateForceDirection(1, aSM.pStats.GravVel, aSM.hookPoint.transform.position, aSM) + RopeLengthOffset(aSM.hookPoint.transform.position, Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position), aSM);
        }
        else{
            aSM.forceDirection = Vector3.zero;
        }

        aSM.moveController.Move(SwingMoveController(aSM));
        if(swingMom != 0){
            aSM.moveController.Move(CalculateMomentumDirection(aSM.pStats.GravVel, aSM.hookPoint.transform.position, aSM));
            swingMom -= .5f;
        }

        if(swingMom<0) swingMom = 0;

        aSM.tempRelease = CalculateSwingReleaseForce();
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }

    //Needs to be Overhauled to properly implement energy loss taking into account players current Velocity, height and input
    //------Important Equations--------
    // TensionForce = Cos(theta) * g * m * Vector3(tensionDirection)
    // Potential Energy = m * g * h -- Will need to modify adding current speed
    // Kinetic Energy = 1/2 * m * V^2 -- assuming no energy loss at the bottom KE = PE at peak we will add energy loss ourselves
    // Total Energy = m * ((g*h) + (1/2 * V^2))
    // Velocity = (2 * ((TotalEnergy/m) - (g*h)))^(1/2) -- This hypothetically could be used for our swing momentum calculation
    // Movement Direction right angle of tension force = Sin(theta) * g * m * Vector3(Right angle to tensionDirection)

    //Calculate the tether direction vector and how much force that vector needs
    Vector3 CalculateForceDirection(float mass, float g, Vector3 hPoint, AerialStateManager aSM){

        //tension direction and angle calculation
        tensionDirection = (hPoint - aSM.transform.position).normalized;
        inclinationAngle = Vector3.Angle((aSM.transform.position - hPoint).normalized, -aSM.transform.up);
        theta = Mathf.Deg2Rad * inclinationAngle;
        if(theta<=.1) theta = 0;

        //How much force the tension needs
        tensionForce = mass * -g * Mathf.Cos(theta);

        //force direction calculation based on tension direction and force
        Vector3 fDirection = tensionDirection * tensionForce;
        return fDirection;
    }

    Vector3 CalculateMomentumDirection(float g, Vector3 hPoint, AerialStateManager aSM){

        tensionMomDirection = (hPoint - aSM.transform.position).normalized;
        hookPointRight = Vector3.Cross(oldXZDir, aSM.transform.up).normalized;
        momDirection = -1 * Vector3.Cross(hookPointRight, tensionMomDirection).normalized;
        
        if(oldXZDir != curXZDir){
            //midpointMom = swingMom;
            swingback = false;
            //Debug.Log("flip");
        }

        if(swingback == false && swingMom <= (oldSwingMom*(.75f))){
            swingback = true;
            oldSwingMom = swingMom;
            oldXZDir = (new Vector3(aSM.hookPoint.transform.position.x,0,aSM.hookPoint.transform.position.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;
            //Debug.Log("swingback");
        }

        curXZDir = (new Vector3(hPoint.x,0,hPoint.z) - new Vector3(aSM.transform.position.x,0,aSM.transform.position.z)).normalized;
        Debug.DrawRay(aSM.transform.position,  momDirection * Time.deltaTime* 100, Color.green);
        return (momDirection * Time.deltaTime * swingMom);
    }

    //Calculates the players initial swing momentum using their height and their current velocity
    float CalculateSwingMom(float playerSpeed, AerialStateManager aSM){

        //Calculate the players height compared to the lowest point in the swing
        float swingHeight = aSM.transform.position.y - (aSM.hookPoint.transform.position.y - ropeLength);
        if(swingHeight <= 1){
            swingHeight = 1;
        }

        float sMom = playerSpeed + (swingHeight*2);
        if(sMom > aSM.maxSwingMom){
            sMom = aSM.maxSwingMom;
        }
        return sMom;
    }
    
    //Special movement for the player while they swing
    Vector3 SwingMoveController(AerialStateManager aSM){

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


        //NEED TO ADD SWINGSPEED EASING
        //this could be just adding a gradual increase in the swing speed instead of using a flat rate
        //Swing movement with swing speed added
        Vector3 swingMovement = (swingDirection * Time.deltaTime * swingSpeed);
        

        return (swingMovement);
    }

    Vector3 RopeLengthOffset(Vector3 hPoint, float curDistance, AerialStateManager aSM){
        
        //How powerful our offset movement has to be
        float offsetPower = ((curDistance - ropeLength) * 200f);

        //The direction we need to apply force to offset when the rope gets lengthened beyond the necessary point
        Vector3 tenDirOffset = (hPoint - aSM.transform.position).normalized;
 

        return tenDirOffset * offsetPower * Time.deltaTime;
    }

    Vector3 CalculateSwingReleaseForce(){

        Vector3 releaseSwingForceDirection = momDirection * ((swingMom) + 10);

        releaseSwingForceDirection = new Vector3(releaseSwingForceDirection.x,0,releaseSwingForceDirection.z);

        if(swingMom < 5){
            return Vector3.zero;
        }
        return releaseSwingForceDirection * Time.deltaTime;
    }
}
