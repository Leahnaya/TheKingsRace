using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LobbyItems : MonoBehaviour
{
    GameObject pPar;
    GameObject player;
    LobbyUI lobbyUI;
    PlayerInventory pInv;
    public GameObject itemOptPrefab;
    private Slider glueGooSlider;

    private PlayerStats pStats;

    InventoryManager invMan;
    public Tooltip tooltip;
    Vector3 position;
    private int pointsLeft;

    //Item Image and Offsets
    public Sprite[] itemBG;
    public Vector3[] itemImgPos;
    public Vector3[] itemImgRot;
    public Vector3[] itemNamePos;
    public Vector3[] itemNameRot;
    public Vector3[] itemCostPos;
    public Vector3[] itemCostRot;
    public Vector3[] itemPinPos;

    ////Runner Body Parts Refs
    public GameObject[] runnerBodyModifiers;
    //back parts
    public GameObject backBattery;
    public GameObject glider;

    //springs refs
    public GameObject leftLeg;
    public GameObject rightLeg;
    public GameObject leftSpring;
    public GameObject rightSpring;

    //shoe refs
    public GameObject leftFoot;
    public GameObject rightFoot;
    public GameObject leftWallShoe;
    public GameObject rightWallShoe;

    //sticky hand 
    public GameObject rightHand;
    public GameObject rightStickyHand;

    //roller skates (may need to raise height)
    public GameObject leftRollerSkate;
    public GameObject rightRollerSkate;

    //tripple jump 
    public GameObject leftQuad;
    public GameObject rightQuad;
    public GameObject thiccLeftQuad;
    public GameObject thiccRightQuad;

    // Start is called before the first frame update
    void Awake(){
        pPar = GameObject.Find("NetworkSMPlayerPrefab");
        player = pPar.transform.Find("T-Pose").gameObject;
        pStats = player.GetComponent<PlayerStats>();
        glueGooSlider = GameObject.Find("GlueGoo").GetComponent<Slider>();
        tooltip = GameObject.Find("ItemTooltip").GetComponent<Tooltip>();
        lobbyUI = this.gameObject.GetComponent<LobbyUI>();
        pInv = player.GetComponent<PlayerInventory>();
        player.GetComponent<MoveStateManager>().enabled = false;
        player.GetComponent<DashStateManager>().enabled = false;
        player.GetComponent<NitroStateManager>().enabled = false;
        player.GetComponent<AerialStateManager>().enabled = false;
        player.GetComponent<OffenseStateManager>().enabled = false;
        pPar.transform.Find("UICam").gameObject.SetActive(false);

        invMan = GetComponent<InventoryManager>();
    }

    void Start(){
        player.GetComponent<CapsuleCollider>().enabled = true;
        InitializeItemB();
        pointsLeft = pStats.PlayerPoints;
        //pointText.text = "Points Left: " + pointsLeft;
    }

    private void InitializeItemB(){
        Debug.Log("Initialize Items");
        int index = 0;
        if(itemOptPrefab != null){
            foreach(var item in invMan.ItemDict){
                Debug.Log("Create Button");
                //Positioning Buttons   
                if(index < 3){
                    position = new Vector3(((index*200)+250),700,0);
                }
                else if(index < 6){
                    position = new Vector3(((index-3)*200)+250,500,0);
                }
                else{
                    position = new Vector3(((index-6)*200)+250,300,0);
                }

                //Creates Button
                var iOpt = Instantiate(itemOptPrefab, position, Quaternion.identity);

                //Uses Item Variables to update button
                iOpt.name = item.Value.itemName;
                iOpt.transform.SetParent(GameObject.Find("Items").transform);
                iOpt.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
                iOpt.transform.GetChild(3).gameObject.SetActive(false);

                //Button Adds item if it can
                iOpt.GetComponent<Button>().onClick.AddListener(delegate { changeBodyParts(item.Value.id, checkAddPart(item.Value.costM, item.Value)); });
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{lobbyUI.EquipItems(item.Value, UpdateObject(item.Value.costM, item.Value, iOpt));});
      
                
                //On Hover display tooltip on exit disable
                EventTrigger.Entry tooltipEntry = new EventTrigger.Entry();
                EventTrigger.Entry tooltipExit = new EventTrigger.Entry();

                tooltipEntry.eventID = EventTriggerType.PointerEnter;
                tooltipExit.eventID = EventTriggerType.PointerExit;

                tooltipEntry.callback.AddListener((data) => {tooltip.ShowTooltip(item.Value.description);});
                tooltipExit.callback.AddListener((data) => {tooltip.HideTooltip();});

                iOpt.GetComponent<EventTrigger>().triggers.Add(tooltipEntry);
                iOpt.GetComponent<EventTrigger>().triggers.Add(tooltipExit);

                //Changes Button Texts
                iOpt.transform.Find("Name").GetComponent<Text>().text = item.Value.itemName;
                iOpt.GetComponent<Image>().sprite = itemBG[index];
                iOpt.transform.Find("Cost").GetComponent<Text>().text = item.Value.costM.ToString();
                iOpt.transform.Find("ItemImg").GetComponent<Image>().sprite = item.Value.itemSprite; // IMPLEMENT WHEN ITEM OBJECT CONTAIN IMAGE REFERENCE

                index++;
            }
        }
        else{
            Debug.Log("itemOption prefab was not set");
        }
        
    }

    private bool UpdateObject(int itemCost, Item item, GameObject button){

        if(pInv.PlayerItemDict.ContainsKey(item.name)){
            //Player can remove the item
            pointsLeft += itemCost;
            
            //Glue level
            glueGooSlider.value += itemCost;

            //Updates pin in note
            button.transform.GetChild(3).gameObject.SetActive(false);

            return true;
        }

        else if(!pInv.PlayerItemDict.ContainsKey(item.name) && (pointsLeft - itemCost) >= 0){
            //Player can add the item
            pointsLeft -= itemCost;

            //glue level
            glueGooSlider.value -= itemCost;

            //Updates pin in note
            button.transform.GetChild(3).gameObject.SetActive(true);

            return true;
        }

        else{
            //Player cannot add or remove the item
            return false;
        }
    }


  
    private int checkAddPart(int itemCost, Item item) {
        //default (no update)
        int tempResult = -1;


        //remove gameobject
        if (pInv.PlayerItemDict.ContainsKey(item.name)) {
            tempResult = 0;
        }
        //add gameObject
        else if (!pInv.PlayerItemDict.ContainsKey(item.name) && (pointsLeft - itemCost) >= 0) {
            tempResult = 1;
        }
        else{
            tempResult = -1;
        }
        return tempResult;
    }
    private void changeBodyParts(int itemID, int addPart){
        //if there is something to update
        if (addPart != -1){
            switch (itemID)
            {
                //Glider
                case 2:
                    //add part
                    if (addPart == 1)
                    {
                        glider.SetActive(true);
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        glider.SetActive(false);
                    }
                    break;
                //grapple
                case 3:
                    //add part
                    if (addPart == 1)
                    {
                        rightStickyHand.SetActive(true);
                        rightHand.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        rightStickyHand.SetActive(false);
                        rightHand.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //nitro
                case 4:
                    ////may use test cse here
                    //add part
                    if (addPart == 1){
                        backBattery.SetActive(true);
                    }
                    //remove part
                    else if(addPart == 0){
                        backBattery.SetActive(false);
                    }
                    break;
                //roller skates
                case 5:
                    //add part
                    if (addPart == 1)
                    {
                        leftRollerSkate.SetActive(true);
                        rightRollerSkate.SetActive(true);
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        leftRollerSkate.SetActive(false);
                        rightRollerSkate.SetActive(false);
                    }
                    break;
                //springs
                case 6:
                    //add part
                    if (addPart == 1)
                    {
                        leftSpring.SetActive(true);
                        rightSpring.SetActive(true);
                        leftLeg.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        rightLeg.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        leftSpring.SetActive(false);
                        rightSpring.SetActive(false);
                        leftLeg.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        rightLeg.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //tripple jump
                case 7:
                    //add part
                    if (addPart == 1)
                    {
                        thiccLeftQuad.SetActive(true);
                        thiccRightQuad.SetActive(true);
                        leftQuad.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        rightQuad.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        thiccLeftQuad.SetActive(false);
                        thiccRightQuad.SetActive(false);
                        leftQuad.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        rightQuad.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //wallrun
                case 8:
                    if (addPart == 1)
                    {
                        leftWallShoe.SetActive(true);
                        rightWallShoe.SetActive(true);
                        leftFoot.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        rightFoot.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        leftWallShoe.SetActive(false);
                        rightWallShoe.SetActive(false);
                        leftFoot.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        rightFoot.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;

            }
        }

    }
}
