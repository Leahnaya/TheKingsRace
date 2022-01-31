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
        pPar = GameObject.Find("DebugPlayerPrefab");
        player1 = pPar.transform.Find("PlayerModel").gameObject;
        pStats = player1.GetComponent<PlayerStats>();
        pointText = GameObject.Find("PlayerPoints").GetComponentInChildren<Text>();
        pInv = player1.GetComponent<PlayerInventory>();
        player1.GetComponent<dPlayerMovement>().enabled = false;
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
            foreach(var item in invMan.ItemDict){
                
                //Positioning Buttons
                if(index < 5){
                    position = new Vector3(((index)),3,0);
                }
                else{
                    position = new Vector3(((index-5)),2,0);
                }

                //Creates Button
                var iOpt = Instantiate(itemOptPrefab, position, Quaternion.identity);

                //Uses Item Variables to update button
                iOpt.name = item.Value.itemName;
                iOpt.transform.SetParent(GameObject.Find("Items").transform);
                iOpt.transform.localScale = new Vector3(1.5f,1.5f,1.5f);

                //Button Adds item if it can
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{pInv.UpdateInventory(item.Value, UpdateObject(item.Value.costM, item.Value, iOpt));});

                //Changes Button Texts
                iOpt.transform.Find("Name").GetComponent<Text>().text = item.Value.itemName;
                iOpt.transform.Find("Cost").GetComponent<Text>().text = item.Value.costM.ToString();
                //iOpt.GetComponentInChildren<Image>() = item.image; // IMPLEMENT WHEN ITEM OBJECT CONTAIN IMAGE REFERENCE

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
            pointText.text = "Points Left: " + pointsLeft;
            button.GetComponent<Image>().color = new Color(1,1,1);

            return true;
        }

        else if(!pInv.PlayerItemDict.ContainsKey(item.name) && (pointsLeft - itemCost) >= 0){
            //Player can add the item
            pointsLeft -= itemCost;
            pointText.text = "Points Left: " + pointsLeft;
            button.GetComponent<Image>().color = new Color(.5f,.5f,.5f);

            return true;
        }

        else{
            //Player cannot add or remove the item
            return false;
        }
    }


    //Enables Script on player
    //NEEDS TO BE CLEANED UP AND IMPROVED
    public void itemToEnable(){
        if(pStats.HasBlink != player1.GetComponent<dBlink>().enabled)
            player1.GetComponent<dBlink>().enabled = pStats.HasBlink;
        if(pStats.HasDash != player1.GetComponent<dDash>().enabled)
            player1.GetComponent<dDash>().enabled = pStats.HasDash;
        if(pStats.HasWallrun != player1.GetComponent<dWallRun>().enabled)
            player1.GetComponent<dWallRun>().enabled = pStats.HasWallrun;
        if(pStats.HasGrapple != player1.GetComponent<dGrapplingHook>().enabled)
            player1.GetComponent<dGrapplingHook>().enabled = pStats.HasGrapple;
        if(pStats.HasNitro != player1.GetComponent<dNitro>().enabled)
            player1.GetComponent<dNitro>().enabled = pStats.HasNitro;
        
    }

    public void nextScene(){
        player1.GetComponent<dPlayerMovement>().enabled = true;
        player1.GetComponent<CapsuleCollider>().enabled = false;
        pPar.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

}
