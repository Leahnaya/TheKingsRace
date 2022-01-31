using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPlace : MonoBehaviour
{

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
    private GameObject PlaceTemp;
    private bool Placing = false;
    private bool HailPlacing = false;
    private bool SlimePlacing = false;

    private void PlaceObject(int ID) {
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Place = Block;
                break;
            case 1:
                Place = Hail;
                break;
            case 2:
                Place = Slime;
                break;
        }
        //Create the object that will follow the Mouse
        PlaceTemp = Instantiate(Place);
        //Layout the grid
        Grid.GetComponent<GridReveal>().GridSwitch(true);

        //The player can then see where the object will be placed, reletive to the Grid
        Placing = true;
    }

    [SerializeField] private Camera KingCam;
    [SerializeField] private LayerMask LayerMask;

    private void FixedUpdate() {
        if (Placing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                PlaceTemp.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonDown(0)) {//Reads the player clicking the left mouse button again
                FindGridBox();
            }
        }
        if (HailPlacing == true) {

        }
        if (SlimePlacing == true) {

        }
    }

    private GameObject Row;
    private void FindGridBox() {
        int BoxSize = 20;
        float i = ((PlaceTemp.transform.position.x) - 101) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((PlaceTemp.transform.position.z) - 626) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + 101;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + 626;//Sets its Z to the Z of the Box
        PlaceTemp.transform.position = new Vector3(x, PlaceTemp.transform.position.y, z);//fully snaps it to the grid
        GridCheck(RowNumb, Box);//Makes sure the Box is a valid position
        if (Place == Slime && SlimePlacing == false) {//If Object is Hail or Slime launch into other functions
            SlimePlc();
        }
        else if (Place == Hail && HailPlacing == false) {
            HailPlc();
        }
    }

    private void GridCheck(int Row, int Box) {
        GameObject RowTrue;
        GameObject BoxTrue;
        RowTrue = Grid.transform.Find("Row" + Row).gameObject;
        if(RowTrue != null) {
            BoxTrue = RowTrue.transform.Find("GridChunk(0," + Box + ")").gameObject;
            if(BoxTrue != null) {
                Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                Placing = false;//Stop placing
            }
        }
    }

    private void SlimePlc() {
        Grid.GetComponent<GridReveal>().GridSwitch(true);
        SlimePlacing = true;
    }

    private void HailPlc() {
        Grid.GetComponent<GridReveal>().GridSwitch(true);
        SlimePlacing = true;
    }

}
