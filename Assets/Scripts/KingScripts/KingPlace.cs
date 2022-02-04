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
        //TODO Spawn thunderstorms on the players
    }


    public GameObject Block;
    public GameObject HailSprite;
    public GameObject HailObject;
    public GameObject Slime;
    private GameObject Grid;
    private GameObject Place;
    private GameObject PlaceTemp;
    private GameObject HailCorner;
    private GameObject SlimeDir;
    private bool FirstPlacing = false;
    private bool HailPlacing = false;
    private bool SlimePlacing = false;
    private int BoxSize = 20;

    void Start()
    {
        Grid = GameObject.FindGameObjectWithTag("KingGrid");
    }

    private void PlaceObject(int ID) {
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Place = Block;
                break;
            case 1:
                Place = HailSprite;
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
        FirstPlacing = true;
    }

    [SerializeField] private Camera KingCam;
    [SerializeField] private LayerMask LayerMask;

    private void FixedUpdate() { //TODO Canceling out an input
        if (FirstPlacing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                PlaceTemp.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonDown(0)) {//Reads the player clicking the left mouse button
                FindGridBox();
            }
        }
        if (HailPlacing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask))
            {
                HailCorner.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonUp(0)) {//Reads the player clicking the left mouse button again
                CreateHail();
            }
        }
        if (SlimePlacing == true) {
            //TODO draw arrow in direction of slime move
            //TODO CreateSlime function?
        }
    }

    private GameObject Row;
    private void FindGridBox() {
        float i = ((PlaceTemp.transform.position.x) - 101) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((PlaceTemp.transform.position.z) - 906) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + 101;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + 906;//Sets its Z to the Z of the Box
        PlaceTemp.transform.position = new Vector3(x, PlaceTemp.transform.position.y, z);//fully snaps it to the grid
        GridCheck(RowNumb, Box, ref FirstPlacing);//Makes sure the Box is a valid position
        if (Place == Slime) {//If Object is Hail or Slime set the respective Placing value to true so it can launch into the secondary placing function
            SlimePlacing = true;
        }
        else if (Place == HailSprite) {
            Grid.GetComponent<GridReveal>().GridSwitch(true);
            HailPlacing = true;
            HailCorner = Instantiate(Place);
        }
    }

    private void GridCheck(int Row, int Box, ref bool Placing) {
        GameObject RowTrue;
        GameObject BoxTrue;
        RowTrue = Grid.transform.Find("Row" + Row).gameObject;
        if(RowTrue != null) {
            BoxTrue = RowTrue.transform.Find("GridChunk" + Box).gameObject;
            if(BoxTrue != null) {
                Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                Placing = false;//Stop placing
            }
        }
    }

    private void CreateHail() {//TODO make sure it makes a box/actually make the Hail Area
        float i = ((HailCorner.transform.position.x) - 101) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((HailCorner.transform.position.z) - 906) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + 101;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + 906;//Sets its Z to the Z of the Box
        HailCorner.transform.position = new Vector3(x, HailCorner.transform.position.y, z);//fully snaps it to the grid
        GridCheck(RowNumb, Box, ref HailPlacing);//Makes sure the Box is a valid position
        //Instanciate HailArea based on PlaceTemp = Top Left and HailCorner = Bottom Right
        if (HailPlacing == false) {
            GameObject Temp;
            Temp = Instantiate(HailObject);
            Temp.GetComponent<HailArea>().SetArea(PlaceTemp.transform.position.x, HailCorner.transform.position.x, PlaceTemp.transform.position.z, HailCorner.transform.position.z);
        }
    }

}
