using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mote: This should be refactored at some point
public class Slide : MonoBehaviour{
    //this may be deprecated at some poiint later in development if we start ussing a rigid bod
    public Camera playerCam;
    private bool isSliding = false;
    private CharacterController charController;
    private PlayerStats playerStats;
    private float orignalTraction;
    private RaycastHit ray;
    private Vector3 up;
    // Start is called before the first frame update
    void Start(){
        
    }
    private void Awake(){
        charController = this.gameObject.GetComponentInParent<CharacterController>();
        playerStats = this.gameObject.GetComponentInParent<PlayerStats>();
        up = this.gameObject.GetComponentInParent<Transform>().up;
    }
    // Update is called once per frame
    void Update(){
        //Debug.DrawRay(this.gameObject.transform.position, up, Color.red, 5.0f);
        //NOTE::
        //if button is held down, start sliding

        if (Input.GetKey(KeyCode.Q)){
            if (isSliding == false){
                orignalTraction = playerStats.Traction;
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = true;
                charController.height = 1.0f;
                playerStats.Traction = 0.01f;
          
            }
        }
        //NOTE: potentialy change this to only allow player back up if there is nothing above them
        if (Input.GetKeyUp(KeyCode.Q)) {
            //if nothing is above the object, stop slidding
            if (Physics.Raycast(this.gameObject.transform.position, up, out ray, 5f) == false)
            {
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = false;
                charController.height = 2.0f;
                playerStats.Traction = orignalTraction;

            }
            else{
                Debug.Log("Object above you");

            }
        }
        //if button is not held down, and still slidding (if they let go but something was above them) check to see if something is still above them, if not 
        else if (Input.GetKey(KeyCode.Q) == false && isSliding==true){
            //if nothing is above the object, stop slidding
            if (Physics.Raycast(this.gameObject.transform.position, up, out ray, 5f) == false)
            {
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = false;
                charController.height = 2.0f;
                playerStats.Traction = orignalTraction;

            }
            else
            {
                Debug.Log("Object above you");

            }

        }
    }
}
