using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dash : MonoBehaviour{
    public Vector3 moveDirection;
 
    public const float maxDashTime = 1.0f;
    public float dashDistance = 10;
    public float dashStoppingSpeed = 0.1f;
    float currentDashTime = maxDashTime;
    float dashSpeed = 12;

    CharacterController characterController;

    void Start(){
        characterController = this.gameObject.GetComponent<CharacterController>();
    }

    //UPDATE CHECK FOR MOVEMENT ONLY WHEN DASHING
    void FixedUpdate(){
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentDashTime = 0;                
        }
        if(currentDashTime < maxDashTime)
        {
            moveDirection = transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        characterController.Move(moveDirection * Time.deltaTime * dashSpeed);
    }

}