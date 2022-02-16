using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class KingPlace : NetworkBehaviour
{

    private bool BoxAvaliable = true;
    private int BoxCool = 3; 
    private bool HailAvaliable = true;
    private int HailCool = 3;
    private bool SlimeAvaliable = true;
    private int SlimeCool = 3;
    private bool ThundAvaliable = true;
    private int ThundCool = 3;

    //Is called when the King clicks on the Block button
    public void OnBlockClicked() {
        if (BoxAvaliable) {
            PlaceObject(0);
        } 
    }

    //Is called when the King clicks on the Hail button
    public void OnHailClicked() {
        if (HailAvaliable) {
            PlaceObject(1);
        }
    }

    //Is called when the King clicks on the Slime button
    public void OnSlimeClicked() {
        if (SlimeAvaliable) {
            PlaceObject(2);
        }
    }

    public GameObject Thunderstorm;
    //Is called when the King clicks on the Thunderstorm button
    public void OnThunderClicked() {
        if (ThundAvaliable) {
            //TODO thunderstorm
            Cooldown(3);
        }
    }

    public GameObject Block;
    public GameObject BlockWithoutNetwork;

    public GameObject HailSprite;
    public GameObject HailObject;
    public GameObject Slime;
    private GameObject Grid;
    private GameObject Place;
    private GameObject PlaceTemp;
    private GameObject HailCorner;
    private int SlimeDir;
    private bool FirstPlacing = false;
    private bool HailPlacing = false;
    private bool SlimePlacing = false;
    private int BoxSize = 20;

    private int Energy = 100;

    private GameObject boxPlaced;

    void Awake()
    {
        Grid = GameObject.FindGameObjectWithTag("KingGrid");
    }

    private void PlaceObject(int ID) {
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Place = BlockWithoutNetwork;
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

    private void Update() { //TODO Canceling out an input
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
            if (Input.GetMouseButtonUp(0)) {//Reads the player releasing the left mouse button
                CreateHail();
                Cooldown(1);
            }
        }
        if (SlimePlacing == true) {
            SlimeDir = DrawArrow();
            if (Input.GetMouseButtonUp(0)) {//Reads the player releasing the left mouse button
                PlaceTemp.GetComponent<Slime>().GooStart(SlimeDir);
                SlimePlacing = false;
                Cooldown(2);
            }
        }
    }

    private void FixedUpdate() {
        CooldownTimer();
    }

    private GameObject Row;
    private void FindGridBox() {
        float i = ((PlaceTemp.transform.position.x) - 101) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((PlaceTemp.transform.position.z) - 906) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + 101;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + 906;//Sets its Z to the Z of the Box

        // Grid Check first for valid location
        GridCheck(RowNumb, Box, ref FirstPlacing);//Makes sure the Box is a valid position

        // Instantiate a networked box at the position
        if (FirstPlacing == false)
        {
            // Calculate the position to spawn at
            Vector3 spawnLoc = new Vector3(x, PlaceTemp.transform.position.y, z);

            PlaceTemp.transform.position = spawnLoc;

            if (Place == BlockWithoutNetwork)
            {
                // Have the server spawn the box
                SpawnBoxServerRPC(spawnLoc);

                // Remove the reference to PlaceTemp
                Destroy(PlaceTemp);
                Cooldown(0);
            }

            if (Place == Slime)
            {//If Object is Hail or Slime set the respective Placing value to true so it can launch into the secondary placing function
                SlimePlacing = true;
            }
            else if (Place == HailSprite)
            {
                Grid.GetComponent<GridReveal>().GridSwitch(true);
                HailPlacing = true;
                HailCorner = Instantiate(Place);
            }
        }
    }

    private void GridCheck(int Row, int Box, ref bool Placing) {//Makes it so items can only be placed in valid positions
        GameObject RowTrue;
        GameObject BoxTrue;
        try {
            RowTrue = Grid.transform.Find("Row" + Row).gameObject;
        } catch {
            RowTrue = null;
        }

        if (RowTrue != null) {
            try {
                BoxTrue = RowTrue.transform.Find("GridChunk" + Box).gameObject;
            } catch {
                BoxTrue = null;
            }
            if (BoxTrue != null) {
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

    private int DrawArrow() {
        Vector3 MosPos = new Vector3(0, 0, 0);
        Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
        if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
            MosPos = RayCastHit.point;
        }
        //TODO Draw arrow
        if (MosPos.x > PlaceTemp.transform.position.x && MosPos.z < PlaceTemp.transform.position.z) {//Temp function to change Slime's dir   //TODO convert mouse position to arrow direction. then return the int of what direction it is
            return 0;//Up
        }
        else if (MosPos.x < PlaceTemp.transform.position.x && MosPos.z < PlaceTemp.transform.position.z) {
            return 1;//Right
        }
        else if (MosPos.x < PlaceTemp.transform.position.x && MosPos.z > PlaceTemp.transform.position.z)  {
            return 2;//Down
        }
        else if (MosPos.x > PlaceTemp.transform.position.x && MosPos.z > PlaceTemp.transform.position.z) {
            return 3;//Left
        }
        else {
            return 2;//Defaults to down, just in case
        }
    }

    private void Cooldown(int ID) {
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                BoxAvaliable = false;//Block Cooldown
                break;
            case 1:
                HailAvaliable = false;//Hail Cooldown
                break;
            case 2:
                SlimeAvaliable = false;//Slime Cooldown
                break;
            case 3:
                ThundAvaliable = false;//Thunderstorm Cooldown
                break;
        }
    }

    private int BSC = 0;
    private int HSC = 0;
    private int SSC = 0;
    private int TSC = 0;  

    private void CooldownTimer() {// Function for tracking the individual cooldowns of the items. It's done in a two-teired system to allow the cooldowns to be eaisly converted as seconds
        if(BoxAvaliable == false) {
            BSC++;
            if (BSC == 50) {
                BSC = 0;
                BoxCool--;
                if (BoxCool == 0) {
                    BoxCool = 3;
                    BoxAvaliable = true;
                }
            }
        }
        if (HailAvaliable == false) {
            HSC++;
            if (HSC == 50) {
                HSC = 0;
                HailCool--;
                if (HailCool == 0) {
                    HailCool = 3;
                    HailAvaliable = true;
                }
            }
        }
        if (SlimeAvaliable == false) {
            SSC++;
            if (SSC == 50) {
                SSC = 0;
                SlimeCool--;
                if (SlimeCool == 0) {
                    SlimeCool = 3;
                    SlimeAvaliable = true;
                }
            }
        }
        if (ThundAvaliable == false) {
            TSC++;
            if (TSC == 50) {
                TSC = 0;
                ThundCool--;
                if (ThundCool == 0) {
                    ThundCool = 3;
                    ThundAvaliable = true;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBoxServerRPC(Vector3 spawnLoc) {
        boxPlaced = Instantiate(Block, spawnLoc, Quaternion.identity);
        boxPlaced.GetComponent<NetworkObject>().Spawn(null, true);
    }
}
