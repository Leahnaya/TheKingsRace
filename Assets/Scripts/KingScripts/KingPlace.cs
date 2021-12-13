using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPlace : MonoBehaviour {

    //Is called when the King clicks on the Block button
    public void OnBlockClicked() {
        PlaceObject(0);
    }
    
    //Is called when the King clicks on the Hail button
    public void OnHailClicked() {
        PlaceObject(1);
    }

    //Is called when the King clicks on the Slime button
    public void OnSlimeClicked() {
        PlaceObject(2);
    }

    public GameObject Thunderstorm;
    //Is called when the King clicks on the Thunderstorm button
    public void OnThundClicked() {

    }


    public GameObject Block;
    public GameObject Hail;
    public GameObject Slime;
    public GameObject Grid;
    private GameObject Place;
    private bool Placing = false;

    private void PlaceObject(int ID) {
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Debug.Log("B");
                Place = Block;
                break;
            case 1:
                Debug.Log("H");
                Place = Hail;
                break;
            case 2:
                Debug.Log("S");
                Place = Slime;
                break;
        }
        //Create the object that will follow the Mouse
        //Instantiate(Place);
        //Layout the grid
        Grid.GetComponent<GridReveal>().GridSwitch(true);

        //The player can then see where the object will be placed, reletive to the Grid
        Placing = true;
    }

    [SerializeField] private Camera KingCam;
    [SerializeField] private LayerMask LayerMask;

    private void Update() {
        if(Placing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                Place.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonUp(1)) {//Reads the player letting up the right mouse button
                //foreach (Transform child in Grid.gameObject.transform) { //Finds the correct Row the cursor is in (Commented out because it's scuffed AF)
                    //if (child.transform.position.z >= Place.transform.position.x && child.transform.position.z - 20 <= Place.transform.position.x)
                    //{
                       // Debug.Log("Help" + child.transform.position.z);
                       //Place the object (Snap it up and to Grid)
                       Instantiate(Place);
                        //break;
                    //}
                Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                Placing = false;//Stop placing
            }
        }
    }
}
