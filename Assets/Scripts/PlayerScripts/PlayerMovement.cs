using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{


    //Variable Section
    /////
    //Speed Variables
    private float minPlayerSpeed = 2f;
    private float maxPlayerSpeed = 20f;
    private float curPlayerSpeed;
    private Vector3 vel;

    //Acceleration Variables
    private float playerAcceleration = .03f;

    //Character Turning /////MAYBE REMOVE WHEN ADD MOUSE CAMERA
    private float turnSpeed = 100.0f;
 

    //Character Moving
    private CharacterController moveController;

    //Jumping
    private float jumpPow = 25f;
    private float gravity = 5.0f;

    //Bump physics
    float mass = 5.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    /////
    

    void Awake(){
        moveController = GetComponent<CharacterController>();
    }

    void Start(){
        curPlayerSpeed = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        //input controls for movement
        InputController();

        //if suffiecient impact magnitude is applied then move player
        if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);

        //Checks if player should respawn
        Respawn();

    }


    //Reads inputs and moves player
    private void InputController(){
        //Keyboard inputs
        Vector3 turnInput = new Vector3(0, Input.GetAxis("HorizontalCam") ,0 );


        //Checks if movement keys have been pressed and calculates correct vector
        Vector3 moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed();
        Vector3 moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed();
        
        //Adds vectors based on movement keys and other conditions to check what the
        //player vector should be under the circumstances
        vel = moveX + moveZ;

        //If space is pressed apply an upwards force to the player
        if(moveController.isGrounded && Input.GetAxis("Jump") != 0){
            AddImpact(transform.up, jumpPow);
        }

        //Gravity
        vel.y -= gravity * Time.deltaTime;

        //////UPDATE SO CAMERA IS ROTATED WITH MOUSE/////////
        //Rotates player with keys
        transform.rotation *= (Quaternion.Euler(turnInput * Time.deltaTime * turnSpeed));

        moveController.Move(vel);
    }
    

    //Calculates speed current player needs to be going
    private float PlayerSpeed(){
        //If nothing is pressed speed is 0
        if(Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f){
            curPlayerSpeed = 0.0f;
            return curPlayerSpeed; 
        }
        //If current speed is below min when pressed set to minimum speed
        else if(curPlayerSpeed < minPlayerSpeed){
            curPlayerSpeed = minPlayerSpeed;
            return minPlayerSpeed;
        }
        // while the speed is below max speed slowly increase it
        else if((curPlayerSpeed >= minPlayerSpeed) && (curPlayerSpeed < maxPlayerSpeed)){
            curPlayerSpeed += playerAcceleration;
            return curPlayerSpeed + playerAcceleration;  
        }
        //If the players speed is above or equal to max speed set speed to max
        else{
            curPlayerSpeed = maxPlayerSpeed;
            return maxPlayerSpeed;
        }
    }

    //Applies impact in a direction with the given force
    public void AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    //Respawns player if they fall below a certain point
    private void Respawn(){
        if(transform.position.y < -1){
            transform.position = new Vector3(1f, 1f, 1f);
        }
    }
}
