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
    private Vector3 moveZ;
    private Vector3 moveX;
    private Vector3 vel;
    private Vector3 driftVel;
    
    //Character Moving
    private CharacterController moveController;

    //Jump value
    private int curJumpNum;
    private bool jumpPressed;

    //Jump physics
    private float mass = 5.0F; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private float distToGround;

    //Camera Variables
    private LayerMask ignoreP;
    private Vector3 camRotation;
    private Camera cam;

    [Range(-45, -15)]
    public int minAngle = -30;
    [Range(30, 80)]
    public int maxAngle = 45;
    [Range(50, 500)]
    public int sensitivity = 200;

    //Blink Variables
    private LineRenderer beam;
    private Vector3 origin;
    private Vector3 endPoint;
    private Vector3 mousePos;
    private RaycastHit hit;
    /////

    void Awake(){
        //Initialize Components
        moveController = GetComponent<CharacterController>();
        pStats = GetComponent<PlayerStats>();
        Cursor.lockState = CursorLockMode.Locked;
        ignoreP = LayerMask.GetMask("Player");

        beam = gameObject.AddComponent<LineRenderer>();
        beam.startWidth = 0.2f;
        beam.endWidth = 0.2f;

        //camera transform
        cam = Camera.main;
    }



    void Start(){
        distToGround = GetComponent<Collider>().bounds.extents.y;

    }

    // Update is called once per frame
    void FixedUpdate(){   
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
        Blink();

    }



    //Reads inputs and moves player
    private void InputController(){
        //Keyboard inputs

        //Checks if movement keys have been pressed and calculates correct vector
        moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed();
        moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed();
        
        //Adds vectors based on movement keys and other conditions to check what the
        //player vector should be under the circumstances
        vel = moveX + moveZ;

        //Gravity
        vel.y -=  pStats.PlayerGrav * Time.deltaTime;

        driftVel = Vector3.Lerp(driftVel, vel, pStats.Traction*Time.deltaTime);
        //Jump Function
        Jump();

        moveController.Move(driftVel);
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
        if(Input.GetAxis("Jump") != 0 && !jumpPressed && curJumpNum+1 < pStats.JumpNum){
            AddImpact(transform.up, pStats.JumpPow);
            curJumpNum++;
            jumpPressed = true;
        }
        
        //NEEDS TO BE MASSIVELY CHANGE LIKELY USE RAYCAST TO CHECK IF ACTUALLY ON GROUND
        //CANNOT USE CHARACTERCONTROLLER.ISGROUNDED IT IS UNRELIABLE
        //If grounded no jumps have been used
        if(IsGrounded()){
            curJumpNum = 0;
        }

        //If space isn't being pressed then jump is false
        if(Input.GetAxis("Jump")==0) jumpPressed = false;
    }

    //Improved IsGrounded
    private bool IsGrounded(){
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, ~ignoreP);
    }

    //Camera
    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        cam.transform.localEulerAngles = camRotation;
    }



    //REMOVE WHEN UNNECCESARY
    //Respawns player if they fall below a certain point
    private void Respawn(){
        if(transform.position.y < -1){
            transform.position = new Vector3(1f, 1f, 1f);
        }
    }



    //ADJUST SO DISTANCE IS DETERMINED BY SCROLL WHEEL
    //blinks the player forwards
    private void Blink(){
        if (Input.GetMouseButton(1)){
            // Finding the origin and end point of laser.
            origin = transform.position + transform.forward * transform.lossyScale.z;

            // Finding mouse pos in 3D space.
            mousePos = Input.mousePosition;
            mousePos.z = 20f;
            endPoint = cam.ScreenToWorldPoint(mousePos);

            // Find direction of beam.
            Vector3 dir = endPoint - origin;
            dir.Normalize();

            // Are we hitting any colliders?
            if (Physics.Raycast(origin, dir, out hit, 20f)){
                // If yes, then set endpoint to hit-point.
                endPoint = hit.point;
            }

            // Set end point of laser.
            beam.SetPosition(0, origin);
            beam.SetPosition(1, endPoint);
            // Draw the laser!
            beam.enabled = true;
            /*Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 5.0f)){
            LineRenderer.SetPosition(1, raycastHit.point);
            }*/

        }

        else if(!Input.GetMouseButton(1) && beam.enabled == true){
            beam.enabled = false;
            //disable character controller for a brief second for teleportation
            //gameObject.GetComponent<CharacterController>().enabled = false;
            //get
            Vector3 bump = new Vector3(0, .5f, 0);
            //if teleporting due to hit to object, bump them a bit outside normal
            if(hit.point != null) {
                transform.position = endPoint + hit.normal * 1.25f;

            }
            //if teleporting in the air or something, just spawn at endpoint
            else{

                transform.position = endPoint;
            }
            //reenable character controller
        }
    }

    
}
