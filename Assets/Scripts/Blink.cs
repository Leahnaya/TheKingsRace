using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Blink : MonoBehaviour{

    // Start is called before the first frame update

    private Camera cam;
    public CharacterController controller;
    private Vector3 origin;
    private Vector3 endPoint;
    private Vector3 mousePos;
    private RaycastHit hit;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start(){

        // Grab the main camera.
        cam = Camera.main;
    }

    private void Update(){
        if (Input.GetMouseButton(1)){

            // Finding the origin and end point of laser.
            origin = this.transform.position + this.transform.forward * this.transform.lossyScale.z;

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
            /*Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 5.0f)){
                LineRenderer.SetPosition(1, raycastHit.point);
            }*/



        }
        if (Input.GetMouseButtonUp(1)){
            //disable character controller for a brief second for teleportation
            //this.gameObject.GetComponent<CharacterController>().enabled = false;
            //get
            Vector3 bump = new Vector3(0, .5f, 0);
            //if teleporting due to hit to object, bump them a bit outside normal
            if(hit.point != null) {
                transform.position = endPoint + hit.normal * 1.25f;
            }
            //if teleporting in the air or something, just spawn at endpoint
            else
            {
                transform.position = endPoint;
            }
            //reenable character controller
            //this.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       

    }
    
}
