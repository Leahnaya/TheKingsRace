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
        //Layout the grid
        Grid.GetComponent<GridReveal>().GridSwitch(true);

        //The player selects where on the grid they want to place the object
        Placing = true;
        /* Center anchored
         * Should be "simple" to snap the cursor poistion onto the grid.
         */

        //The object is placed there
        //Energy is spent
    }

    [SerializeField] private Camera KingCam;
    [SerializeField] private LayerMask LayerMask;

    private void Update() {
        if(Placing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                Place.transform.position = RayCastHit.point;
            }
        }
    }
}
