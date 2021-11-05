using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;



public class dBlink : NetworkBehaviour{

    // Start is called before the first frame update

    private Camera cam;
    public CharacterController controller;
    //Blink Variables
    private LineRenderer beam;
    private Vector3 origin;
    private Vector3 endPoint;
    private Vector3 mousePos;
    private RaycastHit hit;

    LayerMask ignoreP;
    /////

    private void Awake()
    {
        beam = gameObject.AddComponent<LineRenderer>();
        beam.startWidth = 0.2f;
        beam.endWidth = 0.2f;
        beam.enabled = false;

        ignoreP = LayerMask.GetMask("Player");

        controller = GetComponent<CharacterController>();
    }
    void Start(){

        // Grab the main camera.
        //camera transform
        cam =  GetComponentInChildren<Camera>();
    }

    //ADJUST SO DISTANCE IS DETERMINED BY SCROLL WHEEL
    //blinks the player forwards
    private void BlinkMove()
    {
        if (!IsLocalPlayer) { return; }

        if (Input.GetMouseButton(1))
        {
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
            if (Physics.Raycast(origin, dir, out hit, 20f))
            {
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

        else if (!Input.GetMouseButton(1) && beam.enabled == true)
        {
            beam.enabled = false;
            //if teleporting due to hit to object, bump them a bit outside normal
            if (hit.point != null)
            {
                transform.position = endPoint + hit.normal * 1.25f;

            }
            //if teleporting in the air or something, just spawn at endpoint
            else
            {

                transform.position = endPoint;
            }
            //reenable character controller
        }
    }
    
}
