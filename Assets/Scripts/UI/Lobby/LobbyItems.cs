using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class LobbyItems : MonoBehaviour
{
    GameObject pPar;
    GameObject player;
    LobbyUI lobbyUI;
    PlayerInventory pInv;
    public GameObject itemOptPrefab;
    public GameObject costPointer;
    public GameObject costPointerText;
    public Vector3[] costPointerPos;
    private Slider glueGooSlider;

    private PlayerStats pStats;

    InventoryManager invMan;
    public Tooltip tooltip;
    Vector3 position;
    private int pointsLeft;

    //Item Image and Offsets
    public Transform[] postItPos;
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

    //Sound fx
    public AudioSource hoverButton;
    public AudioSource selectButton;

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

                position = postItPos[index].position;

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

                iOpt.GetComponent<Button>().onClick.AddListener(delegate { selectButton.Play(); });//Button play sound on click
      
                
                //On Hover display tooltip on exit disable
                EventTrigger.Entry tooltipEntry = new EventTrigger.Entry();
                EventTrigger.Entry tooltipExit = new EventTrigger.Entry();

                tooltipEntry.eventID = EventTriggerType.PointerEnter;
                tooltipExit.eventID = EventTriggerType.PointerExit;

                tooltipEntry.callback.AddListener((data) => {tooltip.ShowTooltip(item.Value.description);});
                tooltipEntry.callback.AddListener((data) => {hoverButton.Play(); }); //Button on hover play Sound
                tooltipExit.callback.AddListener((data) => {tooltip.HideTooltip();});

                iOpt.GetComponent<EventTrigger>().triggers.Add(tooltipEntry);
                iOpt.GetComponent<EventTrigger>().triggers.Add(tooltipExit);

                //Changes Button Texts
                iOpt.GetComponent<Image>().sprite = itemBG[index];
                iOpt.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.Value.itemName;
                iOpt.transform.Find("Name").localPosition = itemNamePos[index];
                iOpt.transform.Find("Name").localEulerAngles = itemNameRot[index];
               
                iOpt.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = item.Value.costM.ToString();
                iOpt.transform.Find("Cost").localPosition = itemCostPos[index];
                iOpt.transform.Find("Cost").localEulerAngles = itemCostRot[index];

                iOpt.transform.Find("ItemImg").GetComponent<Image>().sprite = item.Value.itemSprite;
                iOpt.transform.Find("ItemImg").localPosition = itemImgPos[index];
                iOpt.transform.Find("ItemImg").localEulerAngles = itemImgRot[index];

                iOpt.transform.Find("Pin").localPosition = itemPinPos[index];  

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
            costPointer.transform.localPosition = costPointerPos[pointsLeft];
            costPointerText.GetComponent<TextMeshProUGUI>().text = pointsLeft.ToString();

            //Glue level
            glueGooSlider.value += itemCost;

            //Updates pin in note
            button.transform.GetChild(3).gameObject.SetActive(false);

            return true;
        }

        else if(!pInv.PlayerItemDict.ContainsKey(item.name) && (pointsLeft - itemCost) >= 0){
            //Player can add the item
            pointsLeft -= itemCost;
            costPointer.transform.localPosition = costPointerPos[pointsLeft];
            costPointerText.GetComponent<TextMeshProUGUI>().text = pointsLeft.ToString();

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
                        runnerBodyModifiers[1].SetActive(true);
                        //glider.SetActive(true);
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[1].SetActive(false);
                        //glider.SetActive(false);
                    }
                    break;
                //grapple
                case 3:
                    //add part
                    if (addPart == 1)
                    {
                        runnerBodyModifiers[11].SetActive(true);
                        runnerBodyModifiers[10].GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[11].SetActive(false);
                        runnerBodyModifiers[10].GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //nitro
                case 4:
                    ////may use test cse here
                    //add part
                    if (addPart == 1){
                        runnerBodyModifiers[0].SetActive(true);
                    }
                    //remove part
                    else if(addPart == 0){
                        runnerBodyModifiers[0].SetActive(false);
                    }
                    break;
                //roller skates
                case 5:
                    //add part
                    if (addPart == 1)
                    {
                        runnerBodyModifiers[12].SetActive(true);
                        runnerBodyModifiers[13].SetActive(true);
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[12].SetActive(false);
                        runnerBodyModifiers[13].SetActive(false);
                    }
                    break;
                //springs
                case 6:
                    //add part
                    if (addPart == 1)
                    {
                        runnerBodyModifiers[4].SetActive(true);
                        runnerBodyModifiers[5].SetActive(true);
                        runnerBodyModifiers[2].GetComponent<SkinnedMeshRenderer>().enabled = false;
                        runnerBodyModifiers[3].GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[4].SetActive(false);
                        runnerBodyModifiers[5].SetActive(false);
                        runnerBodyModifiers[2].GetComponent<SkinnedMeshRenderer>().enabled = true;
                        runnerBodyModifiers[3].GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //tripple jump
                case 7:
                    //add part
                    if (addPart == 1)
                    {
                        runnerBodyModifiers[16].SetActive(true);
                        runnerBodyModifiers[17].SetActive(true);
                        runnerBodyModifiers[14].GetComponent<SkinnedMeshRenderer>().enabled = false;
                        runnerBodyModifiers[15].GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[16].SetActive(false);
                        runnerBodyModifiers[17].SetActive(false);
                        runnerBodyModifiers[14].GetComponent<SkinnedMeshRenderer>().enabled = true;
                        runnerBodyModifiers[15].GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;
                //wallrun
                case 8:
                    if (addPart == 1)
                    {
                        runnerBodyModifiers[8].SetActive(true);
                        runnerBodyModifiers[9].SetActive(true);
                        runnerBodyModifiers[6].GetComponent<SkinnedMeshRenderer>().enabled = false;
                        runnerBodyModifiers[7].GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    //remove part
                    else if (addPart == 0)
                    {
                        runnerBodyModifiers[8].SetActive(false);
                        runnerBodyModifiers[9].SetActive(false);
                        runnerBodyModifiers[6].GetComponent<SkinnedMeshRenderer>().enabled = true;
                        runnerBodyModifiers[7].GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    break;

            }
        }

    }
}
