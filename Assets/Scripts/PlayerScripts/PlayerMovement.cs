using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    
    //Scripts
    public PlayerStats pStats;

    //Variable Section
    /////
    //Speed Variables
    private Vector3 vel;

    //Character Moving
    private CharacterController moveController;

    //Jump value
    private int curJumpNum;
    private bool jumpPressed;

    //Bump physics
    float mass = 5.0F; // defines the character mass
    Vector3 impact = Vector3.zero;
    
    //Camera Variables
    private Vector3 camRotation;
    private Transform cam;

    [Range(-45, -15)]
    public int minAngle = -30;
    [Range(30, 80)]
    public int maxAngle = 45;
    [Range(50, 500)]
    public int sensitivity = 200;
    /////

    void Awake(){
        //Initialize Components
        moveController = GetComponent<CharacterController>();
        pStats = GetComponent<PlayerStats>();
        Cursor.lockState = CursorLockMode.Locked;
        //camera transform
        cam = Camera.main.transform;
    }

    void Start(){
        InitializeStats();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        //input controls for movement
        InputController();

        //Controls for camera
        Rotate();

        //if suffiecient impact magnitude is applied then move player
        if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);

        //Checks if player should respawn
        Respawn();

    }
    
    //REMOVE OR ADJUST ONCE INVENTORY IS IMPLEMENTED
    //Initializes variables for player when inventory is implemented it would be set there
    private void InitializeStats(){
        pStats.MaxVel = 30.0f;
        pStats.MinVel = 2.0f;
        pStats.CurVel = 0.0f;
        pStats.Acc = 0.06f;
        pStats.JumpPow = 100.0f;
        pStats.JumpNum = 50;///SET IT LIKE THIS BC OF THE ISSUE WITH ISGROUNDED
        pStats.Traction = 3.0f;
        pStats.PlayerGrav= 7.0f;
    }

    //Reads inputs and moves player
    private void InputController(){
        //Keyboard inputs

        //Checks if movement keys have been pressed and calculates correct vector
        Vector3 moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed();
        Vector3 moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed();
        
        //Adds vectors based on movement keys and other conditions to check what the
        //player vector should be under the circumstances
        vel = moveX + moveZ;

        if(moveController.isGrounded) curJumpNum = 0;
        //Jump Function
        Jump();

        //Gravity
        vel.y -= pStats.PlayerGrav * Time.deltaTime;

        moveController.Move(vel);
    }
    

    //Calculates speed current player needs to be going
    private float PlayerSpeed(){
        //If nothing is pressed speed is 0
        if(Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f){
            pStats.CurVel = 0.0f;
            return pStats.CurVel; 
        }
        //If current speed is below min when pressed set to minimum speed
        else if(pStats.CurVel < pStats.MinVel){
            pStats.CurVel = pStats.MinVel;
            return pStats.MinVel;
        }
        // while the speed is below max speed slowly increase it
        else if((pStats.CurVel >= pStats.MinVel) && (pStats.CurVel < pStats.MaxVel)){
            pStats.CurVel += pStats.Acc;
            return pStats.CurVel;  
        }
        //If the players speed is above or equal to max speed set speed to max
        else{
            pStats.CurVel = pStats.MaxVel;
            return pStats.CurVel;
        }
    }

    //Applies impact in a direction with the given force
    public void AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    //Jump Function
    private void Jump(){
        //If space is pressed apply an upwards force to the player
        if(Input.GetAxis("Jump") != 0 && !jumpPressed && curJumpNum < pStats.JumpNum){
            AddImpact(transform.up, pStats.JumpPow);
            curJumpNum++;
            jumpPressed = true;
        }
        
        //NEEDS TO BE MASSIVELY CHANGE LIKELY USE RAYCAST TO CHECK IF ACTUALLY ON GROUND
        //CANNOT USE CHARACTERCONTROLLER.ISGROUNDED IT IS UNRELIABLE
        //If grounded no jumps have been used
        if(moveController.isGrounded) curJumpNum = 0;

        //If space isn't being pressed then jump is false
        if(Input.GetAxis("Jump")==0) jumpPressed = false;
    }

    //Camera
    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        cam.localEulerAngles = camRotation;
    }

    //REMOVE WHEN UNNECCESARY
    //Respawns player if they fall below a certain point
    private void Respawn(){
        if(transform.position.y < -1){
            transform.position = new Vector3(1f, 1f, 1f);
        }
    }

    
}
