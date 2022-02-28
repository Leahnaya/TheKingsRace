using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class KingPlace : NetworkBehaviour
{

    public GameObject Block;
    public GameObject BlockWithoutNetwork;

    public GameObject HailSprite;
    public GameObject HailObject;
    public GameObject Slime;
    private GameObject Grid;
    public GameObject Panel;
    private GameObject Place;
    private GameObject PlaceTemp = null;
    private GameObject HailCorner;
    
    public bool FirstPlacing = false;
    private bool HailPlacing = false;
    private bool SlimePlacing = false;

    private int SlimeDir;
    private int BoxSize = 20;

    private KingAbility KABlock = new KingAbility(2, 5);
    private KingAbility KASlime = new KingAbility(2, 15);
    private KingAbility KAHail = new KingAbility(2, 15);
    private KingAbility KAThund = new KingAbility(2, 10);

    public GameObject Thunderstorm;
    //Is called when the King clicks on the Thunderstorm button
    public void OnThunderClicked()
    {
        if (KAThund.IsAvaliable()) {
            //TODO thunderstorm
            KAThund.UseItem();
            MenuOff();
        }
    }

    private int Energy = 100;

    private GameObject boxPlaced;

    void Awake()
    {
        Grid = GameObject.FindGameObjectWithTag("KingGrid");
    }

    bool Avaliable;
    public void PlaceObject(int ID) { //Is called when trhe King clicks the Block,Hail,or Slime button
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        Avaliable = false;
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Place = BlockWithoutNetwork;
                Avaliable = KABlock.IsAvaliable();
                break;
            case 1:
                Place = HailSprite;
                Avaliable = KAHail.IsAvaliable();
                break;
            case 2:
                Place = Slime;
                Avaliable = KASlime.IsAvaliable();
                break;
        }
        if (Avaliable == true) {
            MenuOff();
            //Create the object that will follow the Mouse
            PlaceTemp = Instantiate(Place);
            //Layout the grid
            Grid.GetComponent<GridReveal>().GridSwitch(true);

            //The player can then see where the object will be placed, reletive to the Grid
            FirstPlacing = true;
        }
    }

    private void MenuOff() {
        Panel.GetComponent<RadialMenu>().MenuOff();
    }

    [SerializeField] private Camera KingCam;
    [SerializeField] private LayerMask LayerMask;

    private void Update() {
        if (FirstPlacing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                PlaceTemp.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonDown(0)) {//Reads the player clicking the left mouse button
                FindGridBox();
            }
            if (Input.GetMouseButtonUp(1)) {//Reads the player pressing the right mouse button
                CancelPlacing();
            }
        }
        if (HailPlacing == true) {
            Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                HailCorner.transform.position = RayCastHit.point;
            }
            if (Input.GetMouseButtonUp(0)) {//Reads the player releasing the left mouse button
                CreateHail();
                KAHail.UseItem();
            }
        }
        if (SlimePlacing == true) {
            SlimeDir = DrawArrow();
            if (Input.GetMouseButtonUp(0)) {//Reads the player releasing the left mouse button
                PlaceTemp.GetComponent<Slime>().GooStart(SlimeDir);
                foreach (Transform child in PlaceTemp.transform) {
                    child.gameObject.SetActive(false);
                }
                SlimePlacing = false;
                KASlime.UseItem();
            }
        }
    }

    public void CancelPlacing() {//Allows the player to cancel out a button press
        Destroy(PlaceTemp);
        Grid.GetComponent<GridReveal>().GridSwitch(false);
        FirstPlacing = false;
    }

    private void FixedUpdate() {
        CooldownTimer();
        EnergyRefill();
    }

    int xOffset = 101;
    int zOffset = 906;
    private GameObject Row;
    private void FindGridBox() {
        float i = ((PlaceTemp.transform.position.x) - xOffset) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((PlaceTemp.transform.position.z) - zOffset) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + xOffset;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + zOffset;//Sets its Z to the Z of the Box

        // Grid Check first for valid location
        GridCheck(RowNumb, Box, ref FirstPlacing);//Makes sure the Box is a valid position

        // Instantiate a networked box at the position
        if (FirstPlacing == false) {
            // Calculate the position to spawn at
            Vector3 spawnLoc = new Vector3(x, PlaceTemp.transform.position.y, z);

            PlaceTemp.transform.position = spawnLoc;

            if (Place == BlockWithoutNetwork) {
                // Have the server spawn the box
                SpawnBoxServerRPC(spawnLoc);

                // Remove the reference to PlaceTemp
                Destroy(PlaceTemp);
                KABlock.UseItem();
            }

            if (Place == Slime) {//If Object is Hail or Slime set the respective Placing value to true so it can launch into the secondary placing function
                SlimePlacing = true;
            }
            else if (Place == HailSprite) {
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
        float i = ((HailCorner.transform.position.x) - xOffset) / -BoxSize; //Finds the Row the cursor is in
        int RowNumb = (int)i; //Rounds it down
        float y = ((HailCorner.transform.position.z) - zOffset) / -BoxSize;  //Finds the Box in the Row the cursor is in
        int Box = (int)y; //Rounds it down
        int x = (RowNumb * -BoxSize) + xOffset;//Sets it's X to the X of the Row
        int z = (Box * -BoxSize) + zOffset;//Sets its Z to the Z of the Box
        HailCorner.transform.position = new Vector3(x, HailCorner.transform.position.y, z);//fully snaps it to the grid
        GridCheck(RowNumb, Box, ref HailPlacing);//Makes sure the Box is a valid position
        //Instanciate HailArea based on PlaceTemp = Top Left and HailCorner = Bottom Right
        if (HailPlacing == false) {
            GameObject Temp;
            Temp = Instantiate(HailObject);
            Temp.GetComponent<HailArea>().SetArea(PlaceTemp.transform.position.x, HailCorner.transform.position.x, PlaceTemp.transform.position.z, HailCorner.transform.position.z);
            Destroy(PlaceTemp);
            Destroy(HailCorner);
        }
    }

    private int DrawArrow() {
        Vector3 MosPos = new Vector3(0, 0, 0);
        Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
        if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
            MosPos = RayCastHit.point;
        }
        foreach (Transform child in PlaceTemp.transform) {
            child.gameObject.SetActive(false);
        }//y=x(PlaceTemp.y - PlaceTemp.x) y=-x(PlaceTemp.y - PlaceTemp.x)
        if (MosPos.x > PlaceTemp.transform.position.x && MosPos.z < PlaceTemp.transform.position.z) {//Temp function to change Slime's dir
            PlaceTemp.transform.Find("ArrowUP").gameObject.SetActive(true);
            return 0;//Up
        }
        else if (MosPos.x < PlaceTemp.transform.position.x && MosPos.z < PlaceTemp.transform.position.z) {
            PlaceTemp.transform.Find("ArrowRIGHT").gameObject.SetActive(true);
            return 1;//Right
        }
        else if (MosPos.x < PlaceTemp.transform.position.x && MosPos.z > PlaceTemp.transform.position.z)  {
            PlaceTemp.transform.Find("ArrowDOWN").gameObject.SetActive(true);
            return 2;//Down
        }
        else if (MosPos.x > PlaceTemp.transform.position.x && MosPos.z > PlaceTemp.transform.position.z) {
            PlaceTemp.transform.Find("ArrowLEFT").gameObject.SetActive(true);
            return 3;//Left
        }
        else {
            return 2;//Defaults to down, just in case
        }
    } 

    private void CooldownTimer() {// Function for tracking the individual cooldowns of the items. It's done in a two-teired system to allow the cooldowns to be eaisly converted as seconds
        KABlock.Cooldown();
        KAHail.Cooldown();
        KASlime.Cooldown();
        KAThund.Cooldown();
    }

    private bool SpendEnergy(KingAbility Ability) {
        if (Energy > Ability.Energy()) {
            Energy -= Ability.Energy();
            return true;
        }
        else {
            return false;
        }
    }

    private void EnergyRefill() {
        if(Energy < 100) {
            Energy++;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBoxServerRPC(Vector3 spawnLoc) {
        boxPlaced = Instantiate(Block, spawnLoc, Quaternion.identity);
        boxPlaced.GetComponent<NetworkObject>().Spawn(null, true);
    }
}
