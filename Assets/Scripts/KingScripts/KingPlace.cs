using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingPlace : NetworkBehaviour
{

    public GameObject Block;
    public GameObject BlockWithoutNetwork;

    public GameObject Slime;
    public GameObject SlimeWithoutNetwork;

    public GameObject Bumper;
    public GameObject BumperWithoutNetwork;

    public GameObject HailSprite;
    public GameObject HailObject;
    private GameObject Grid;
    private GameObject MountGrid;
    public GameObject Panel;
    private GameObject Place;
    private GameObject PlaceTemp = null;
    private GameObject HailCorner;
    
    public bool FirstPlacing = false;
    private bool HailPlacing = false;
    private bool SlimePlacing = false;

    private int SlimeDir;
    private GameObject SlimeBody = null;

    private int BoxSize = 20;
    private Vector3 MontStr = new Vector3(-4000, 300, 495);

    private KingAbility KABlock = new KingAbility(2, 5);
    private KingAbility KASlime = new KingAbility(3, 15);
    private KingAbility KAHail = new KingAbility(4, 15);
    private KingAbility KABump = new KingAbility(6, 10);


    private float[] MountPlats = { 302f, 314f, 329f, 344f, 359f, 399f, 414f, 429f, 469f, 484f, 499f, 514f};
    private float[] MountxOffsets = { -50f, -73.4f, -100f, -127f, -134.7f, -130f, -34f, 14.5f, 91.2f, 129f, 129.5f };
    private float[] MountzOffsets = { 130f, 120f, 97f, 56.6f, 32f, -50f, -135.5f, -138.5f, -105f, -52.5f, 50f };


    private int Energy = 100;

    private GameObject placedObj;

    void Start() {
        KABlock.KingButton(Panel.transform.GetChild(0).gameObject);//Grabs each button from the 
        KASlime.KingButton(Panel.transform.GetChild(2).gameObject);
        KAHail.KingButton(Panel.transform.GetChild(1).gameObject);
        KABump.KingButton(Panel.transform.GetChild(3).gameObject);

        // Only try to grab references if we are actually in the game scene
        if (SceneManager.GetActiveScene().buildIndex == 3) {
            Grid = GameObject.FindGameObjectWithTag("KingGrid");
            MountGrid = GameObject.FindGameObjectWithTag("MountainGrid");
            try
            {
                Grid.GetComponent<GridReveal>().ToggleMountainGrid(false);
            } catch(NullReferenceException e)
            {
                Debug.LogError(e);
            }
        }
    }

    bool Avaliable;
    public void PlaceObject(int ID) { //Is called when trhe King clicks the Block,Hail,or Slime button
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime,ID-3 = Bumper
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
                Place = SlimeWithoutNetwork;
                Avaliable = KASlime.IsAvaliable();
                break;
            case 3:
                Place = BumperWithoutNetwork;
                Avaliable = KABump.IsAvaliable();
                break;
        }
        if (Avaliable == true) {
            MenuOff();
            Panel.GetComponent<RadialMenu>().PlaceObj(ID);
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
            Ray Ray;
            if (this.transform.Find("kingCursor").gameObject.activeInHierarchy)
            {
                Ray = KingCam.ScreenPointToRay(this.transform.Find("kingCursor").gameObject.transform.position);//Raycast to find the point where the mouse cursor is
            }
            else
            {
                Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            }
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
            if (this.transform.Find("kingCursor").gameObject.activeInHierarchy)
            {
                Ray = KingCam.ScreenPointToRay(this.transform.Find("kingCursor").gameObject.transform.position);//Raycast to find the point where the mouse cursor is
            }
            else
            {
                Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
            }
            if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
                HailCorner.transform.position = RayCastHit.point;
                HailCorner.GetComponent<LineRenderer>().SetPosition(1, new Vector3(HailCorner.transform.position.x, HailCorner.transform.position.y + 35, PlaceTemp.transform.position.z));
                HailCorner.GetComponent<LineRenderer>().SetPosition(2, new Vector3(HailCorner.transform.position.x, HailCorner.transform.position.y + 35, HailCorner.transform.position.z));
                HailCorner.GetComponent<LineRenderer>().SetPosition(3, new Vector3(PlaceTemp.transform.position.x, HailCorner.transform.position.y + 35, HailCorner.transform.position.z));
            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.JoystickButton0)) {//Reads the player releasing the left mouse button
                CreateHail();
                MenuOff();
                KAHail.UseItem();
            }
        }
        if (SlimePlacing == true) {
            SlimeDir = DrawArrow();
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.JoystickButton0)) {//Reads the player releasing the left mouse button

                Vector3 slimeLoc = PlaceTemp.transform.position;//Gets the Slime's position and rotation
                Quaternion slimeRot = PlaceTemp.transform.rotation;
                Quaternion slimeBodyRot = SlimeBody.transform.rotation;

                Destroy(PlaceTemp);

                // PLACE NETWORKED VERSION
                SpawnSlimeServerRPC(slimeLoc, slimeRot, slimeBodyRot, SlimeDir);

                SlimePlacing = false;
                MenuOff();
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
    int boxsize = 20;
    float MountxOffset = -50f;
    float MountzOffset = 130f;
    private GameObject Row;
    private GameObject Platform;
    Vector3 spawnLoc;
    Quaternion spawnRot = Quaternion.identity;
    private void FindGridBox() {
        int RowNumb = 0, Box = 0, x = 0, z = 0;
        if (PlaceTemp.transform.position.x < MontStr.x) {//King is trying to place on the mountain
            Platform = null;
            bool found = false;
            for(int i = 0; i < MountPlats.Length-1; i++) {
                if (MountPlats[i] <= PlaceTemp.transform.position.y && MountPlats[i+1] >= PlaceTemp.transform.position.y) {//Finds the platform by comparing y values
                    Platform = MountGrid.transform.Find("Platform" + i).gameObject;//Sets the platform and offsets for that platform
                    MountxOffset = MountxOffsets[i];
                    MountzOffset = MountzOffsets[i];
                    break;//Breaks out when it finds the platform
                }
            }
            if (Platform != null) {//Makes sure it found a platform, so there are no null refs
                foreach (Transform row in Platform.transform) { //Looks through every row on the platform
                    foreach (Transform box in row) {//Looks at every Box in the row
                        if (PlaceTemp.transform.position.x <= (box.transform.position.x-MountxOffset) + boxsize / 2 && PlaceTemp.transform.position.x >= (box.transform.position.x-MountxOffset) - boxsize / 2) {//Sees if the x fits within the box
                            if (PlaceTemp.transform.position.z <= (box.transform.position.z-MountzOffset) + boxsize / 2 && PlaceTemp.transform.position.z >= (box.transform.position.z-MountzOffset) - boxsize / 2) {//Sees if the z fits within the box
                                found = true;
                                Vector3 rot = row.transform.rotation.eulerAngles;
                                PlaceTemp.transform.Rotate(0, (rot.y-90), 0);//Sets the object's rotation to the rows rotation
                                spawnRot = PlaceTemp.transform.rotation;
                                spawnLoc = new Vector3(box.transform.position.x-MountxOffset, PlaceTemp.transform.position.y, box.transform.position.z-MountzOffset);//Sets the object's location
                                break;//Breaks out from the loops when found
                            }
                        }
                    }
                    if (found == true) {//Stops it from going through all the rows
                        Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                        FirstPlacing = false;
                        break;
                    }
                }
            }
        }
        else {
            float i = ((PlaceTemp.transform.position.x) - xOffset) / -BoxSize; //Finds the Row the cursor is in
            RowNumb = (int)i; //Rounds it down
            float y = ((PlaceTemp.transform.position.z) - zOffset) / -BoxSize;  //Finds the Box in the Row the cursor is in
            Box = (int)y; //Rounds it down
            x = (RowNumb * -BoxSize) + xOffset;//Sets it's X to the X of the Row
            z = (Box * -BoxSize) + zOffset;//Sets its Z to the Z of the Box
            // Grid Check first for valid location
            GridCheck(RowNumb, Box, ref FirstPlacing);//Makes sure the Box is a valid position
            // Calculate the position to spawn at
            spawnLoc = new Vector3(x, PlaceTemp.transform.position.y, z);
        }

        // Instantiate a networked box at the position
        if (FirstPlacing == false) {     

            PlaceTemp.transform.position = spawnLoc;

            if (Place == BlockWithoutNetwork) {
                // Have the server spawn the box
                SpawnBoxServerRPC(spawnLoc, spawnRot);

                // Remove the reference to PlaceTemp
                Destroy(PlaceTemp);
                MenuOff();
                KABlock.UseItem();
            }

            if (Place == BumperWithoutNetwork) {
                //Server spawn Bumper
                SpawnBumperServerRPC(spawnLoc, spawnRot);

                //Remove PlaceTemp
                Destroy(PlaceTemp);
                MenuOff();
                KABump.UseItem();
            }

            if (Place == SlimeWithoutNetwork) {//If Object is Hail or Slime set the respective Placing value to true so it can launch into the secondary placing function
                SlimePlacing = true;
            }
            else if (Place == HailSprite) {
                Grid.GetComponent<GridReveal>().GridSwitch(true);
                HailPlacing = true;
                HailCorner = Instantiate(Place);
                HailCorner.GetComponent<LineRenderer>().SetPosition(0, new Vector3(PlaceTemp.transform.position.x, PlaceTemp.transform.position.y + 35, PlaceTemp.transform.position.z));
            }
        }
    }

    [SerializeField] private LayerMask GroundMask;

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
                //Check if there's something in the box, unless it's Hail, then it skips past
                if (Physics.CheckSphere(BoxTrue.transform.GetChild(0).transform.position, BoxSize-3f, GroundMask) == false || PlaceTemp == HailSprite || Placing == HailPlacing)
                {
                    Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                    Placing = false;//Stop placing
                }
            }
        }
    }

    private void CreateHail() {
        if (PlaceTemp.transform.position.x < MontStr.x)
        {//King is trying to place on the mountain
            Platform = null;
            bool found = false;
            for (int i = 0; i < MountPlats.Length-1; i++) {
                if (MountPlats[i] <= PlaceTemp.transform.position.y && MountPlats[i + 1] >= PlaceTemp.transform.position.y)
                {//Finds the platform by comparing y values
                    Platform = MountGrid.transform.Find("Platform" + i).gameObject;//Sets the platform and offsets for that platform
                    MountxOffset = MountxOffsets[i];
                    MountzOffset = MountzOffsets[i];
                    break;//Breaks out when it finds the platform
                }
            }
            if (Platform != null)
            {//Makes sure it found a platform, so there are no null refs
                foreach (Transform row in Platform.transform)
                { //Looks through every row on the platform
                    foreach (Transform box in row)
                    {//Looks at every Box in the row
                        if (HailCorner.transform.position.x <= (box.transform.position.x - MountxOffset) + boxsize / 2 && HailCorner.transform.position.x >= (box.transform.position.x - MountxOffset) - boxsize / 2)
                        {//Sees if the x fits within the box
                            if (HailCorner.transform.position.z <= (box.transform.position.z - MountzOffset) + boxsize / 2 && HailCorner.transform.position.z >= (box.transform.position.z - MountzOffset) - boxsize / 2)
                            {//Sees if the z fits within the box
                                found = true;
                                HailCorner.transform.position = new Vector3(box.transform.position.x - MountxOffset, HailCorner.transform.position.y, box.transform.position.z - MountzOffset);//Sets the object's location
                                break;//Breaks out from the loops when found
                            }
                        }
                    }
                    if (found == true)
                    {//Stops it from going through all the rows
                        Grid.GetComponent<GridReveal>().GridSwitch(false); //Switch off the grid
                        HailPlacing = false;
                        break;
                    }
                }
            }
        }
        else
        {
            float i = ((HailCorner.transform.position.x) - xOffset) / -BoxSize; //Finds the Row the cursor is in
            int RowNumb = (int)i; //Rounds it down
            float y = ((HailCorner.transform.position.z) - zOffset) / -BoxSize;  //Finds the Box in the Row the cursor is in
            int Box = (int)y; //Rounds it down
            int x = (RowNumb * -BoxSize) + xOffset;//Sets it's X to the X of the Row
            int z = (Box * -BoxSize) + zOffset;//Sets its Z to the Z of the Box
            HailCorner.transform.position = new Vector3(x, HailCorner.transform.position.y, z);//fully snaps it to the grid
            GridCheck(RowNumb, Box, ref HailPlacing);//Makes sure the Box is a valid position
        }
        //Instanciate HailArea based on PlaceTemp = Top Left and HailCorner = Bottom Right
        if (HailPlacing == false) {     
            Vector4 spawnLoc = new Vector4(PlaceTemp.transform.position.x, HailCorner.transform.position.x, PlaceTemp.transform.position.z, HailCorner.transform.position.z);
            SpawnHailServerRPC(spawnLoc);

            Destroy(PlaceTemp);
            Destroy(HailCorner);
        }
    }

    private int DrawArrow() { //TODO Fix how it determines where to point
        Vector3 MosPos = new Vector3(0, 0, 0);
        GameObject Arrow = null;
        int DirNumber = 2;//Defaults to down, just in case

        Ray Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
        if (this.transform.Find("kingCursor").gameObject.activeInHierarchy)
        {
            Ray = KingCam.ScreenPointToRay((this.transform.Find("kingCursor").gameObject.transform.position));//Raycast to find the point where the mouse cursor is
        }
        else
        {
            Ray = KingCam.ScreenPointToRay(Input.mousePosition);//Raycast to find the point where the mouse cursor is
        }

        if (Physics.Raycast(Ray, out RaycastHit RayCastHit, float.MaxValue, LayerMask)) {
        MosPos = RayCastHit.point;
        }
        foreach (Transform child in PlaceTemp.transform) {
            if (child.gameObject.tag == "Arrow") { // Makes sure not to turn off the model
                child.gameObject.SetActive(false);
            }
            else
            {
                SlimeBody = child.gameObject;
            }
        }

        Vector3 Dir = (MosPos - PlaceTemp.transform.position).normalized;//Finds the direction to the Mouse

        if (Dir.z <= -.8f) {//Set of If statements that determine the direction of the Slime, relative to the Mouse's Position
            Arrow = PlaceTemp.transform.Find("ArrowUP").gameObject;
            DirNumber = 0;//Up
        }
        else if (Dir.x <= -.8f) {
            Arrow = PlaceTemp.transform.Find("ArrowRIGHT").gameObject;
            DirNumber = 1;//Right
        }
        else if (Dir.z >= .8f) {
            Arrow = PlaceTemp.transform.Find("ArrowDOWN").gameObject;
            DirNumber = 2;//Down
        }
        else if (Dir.x >= .8f) {
            Arrow = PlaceTemp.transform.Find("ArrowLEFT").gameObject;
            DirNumber = 3;//Left
        }
        else {
            Arrow = PlaceTemp.transform.Find("ArrowDOWN").gameObject; //Defaults to down
        }

        Arrow.SetActive(true);
        SlimeBody.transform.LookAt(Arrow.transform);
        SlimeBody.transform.Rotate(0, -90, -5);
        return DirNumber;
    } 

    private void CooldownTimer() {// Function for tracking the individual cooldowns of the items. It's done in a two-teired system to allow the cooldowns to be eaisly converted as seconds
        KABlock.Cooldown();
        KAHail.Cooldown();
        KASlime.Cooldown();
        KABump.Cooldown();
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


    //ServerRPCs for spawning the King's abilities
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBoxServerRPC(Vector3 spawnLoc, Quaternion spawnRot) {
        placedObj = Instantiate(Block, spawnLoc, spawnRot);
        placedObj.GetComponent<NetworkObject>().Spawn(null, true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBumperServerRPC(Vector3 spawnLoc, Quaternion spawnRot) {
        placedObj = Instantiate(Bumper, spawnLoc, spawnRot);
        placedObj.GetComponent<NetworkObject>().Spawn(null, true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnSlimeServerRPC(Vector3 spawnLoc, Quaternion spawnRot, Quaternion slimeBodyRot, int SlimeDir) {
        placedObj = Instantiate(Slime, spawnLoc, spawnRot);
        placedObj.transform.GetChild(0).rotation = slimeBodyRot;
        placedObj.GetComponent<Slime>().GooStart(SlimeDir);

        placedObj.GetComponent<NetworkObject>().Spawn(null, true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnHailServerRPC(Vector4 spawnLoc) {
        placedObj = Instantiate(HailObject, Vector3.zero, Quaternion.identity);

        placedObj.GetComponent<HailArea>().SetArea(spawnLoc.x, spawnLoc.y, spawnLoc.z, spawnLoc.w);

        placedObj.GetComponent<NetworkObject>().Spawn(null, true);
    }
}
