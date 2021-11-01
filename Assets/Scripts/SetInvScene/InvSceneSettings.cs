using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InvSceneSettings : MonoBehaviour
{
    GameObject pPar;
    GameObject player1;
    PlayerInventory pInv;
    public GameObject itemOptPrefab;
    private Text pointText;
    private PlayerStats pStats;

    InventoryManager invMan; 
    Vector3 position;
    private int pointsLeft;

    // Start is called before the first frame update
    void Awake(){
        pPar = GameObject.Find("PlayerPrefab");
        player1 = pPar.transform.Find("PlayerModel").gameObject;
        pStats = player1.GetComponent<PlayerStats>();
        pointText = GameObject.Find("PlayerPoints").GetComponentInChildren<Text>();
        pInv = player1.GetComponent<PlayerInventory>();
        player1.GetComponent<PlayerMovement>().enabled = false;
        pPar.transform.Find("PlayerCam").gameObject.SetActive(false);

        invMan = GetComponent<InventoryManager>();
    }

    void Start(){
        player1.GetComponent<CapsuleCollider>().enabled = true;
        InitializeItemB();
        pointsLeft = pStats.PlayerPoints;
        pointText.text = "Points Left: " + pointsLeft;
    }

    void FixedUpdate(){
        itemToEnable();
    }

    private void InitializeItemB(){
        int index = 0;
        if(itemOptPrefab != null){
            foreach(Item item in invMan.ItemList){
                
                if(index < 5){
                    position = new Vector3(((index)),3,0);
                }
                else{
                    position = new Vector3(((index-5)),2,0);
                }
                var iOpt = Instantiate(itemOptPrefab, position, Quaternion.identity);

                iOpt.name = item.itemName;
                iOpt.transform.SetParent(GameObject.Find("Items").transform);
                iOpt.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{pInv.AddItem(item, UpdateObject(item.costM, item, iOpt));});
                iOpt.transform.Find("Name").GetComponent<Text>().text = item.itemName;
                iOpt.transform.Find("Cost").GetComponent<Text>().text = item.costM.ToString();

                //iOpt.GetComponentInChildren<Image>() = item.image; // IMPLEMENT WHEN ITEM OBJECT CONTAIN IMAGE REFERENCE
                index++;
            }
        }
        else{
            Debug.Log("itemOption prefab was not set");
        }
        
    }

    private bool UpdateObject(int itemCost, Item item, GameObject button){
        if(pInv.GetItems().Contains(item)){
            pointsLeft += itemCost;
            pointText.text = "Points Left: " + pointsLeft;
            button.GetComponent<Image>().color = new Color(1,1,1);
            return true;
        }
        else if(!pInv.GetItems().Contains(item) && (pointsLeft - itemCost) >= 0){
            pointsLeft -= itemCost;
            pointText.text = "Points Left: " + pointsLeft;
            button.GetComponent<Image>().color = new Color(.5f,.5f,.5f);
            return true;
        }
        else{
            return false;
        }
    }


    //Enables Script on player
    //NEEDS TO BE CLEANED UP AND IMPROVED
    public void itemToEnable(){
        if(pStats.HasBlink != player1.GetComponent<Blink>().enabled)
            player1.GetComponent<Blink>().enabled = pStats.HasBlink;
        if(pStats.HasDash != player1.GetComponent<Dash>().enabled)
            player1.GetComponent<Dash>().enabled = pStats.HasDash;
        if(pStats.HasWallrun != player1.GetComponent<WallRun>().enabled)
            player1.GetComponent<WallRun>().enabled = pStats.HasWallrun;
        if(pStats.HasGrapple != player1.GetComponent<GrapplingHook>().enabled)
            player1.GetComponent<GrapplingHook>().enabled = pStats.HasGrapple;
        if(pStats.HasNitro != player1.GetComponent<Nitro>().enabled)
            player1.GetComponent<Nitro>().enabled = pStats.HasNitro;
        
    }

    public void nextScene(){
        player1.GetComponent<PlayerMovement>().enabled = true;
        player1.GetComponent<CapsuleCollider>().enabled = false;
        pPar.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

}
