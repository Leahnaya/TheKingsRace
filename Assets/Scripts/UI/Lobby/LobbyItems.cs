using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    Vector3 position;
    private int pointsLeft;

    

    // Start is called before the first frame update
    void Awake(){
        pPar = GameObject.Find("PlayerPrefab");
        player = pPar.transform.Find("PlayerModel").gameObject;
        pStats = player.GetComponent<PlayerStats>();
        glueGooSlider = GameObject.Find("GlueGoo").GetComponent<Slider>();
        lobbyUI = this.gameObject.GetComponent<LobbyUI>();
        pInv = player.GetComponent<PlayerInventory>();
        player.GetComponent<PlayerMovement>().enabled = false;
        pPar.transform.Find("PlayerCam").gameObject.SetActive(false);

        invMan = GetComponent<InventoryManager>();
    }

    void Start(){
        player.GetComponent<CapsuleCollider>().enabled = true;
        InitializeItemB();
        pointsLeft = pStats.PlayerPoints;
        //pointText.text = "Points Left: " + pointsLeft;
    }

    private void InitializeItemB(){
        int index = 0;
        if(itemOptPrefab != null){
            foreach(var item in invMan.ItemDict){
                
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
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{lobbyUI.EquipItems(item.Value, UpdateObject(item.Value.costM, item.Value, iOpt));});

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

}
