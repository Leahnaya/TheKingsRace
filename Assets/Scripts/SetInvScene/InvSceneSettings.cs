using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InvSceneSettings : MonoBehaviour
{
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
        player1 = GameObject.Find("PlayerPrefab");
        pStats = player1.GetComponent<PlayerStats>();
        pointText = GameObject.Find("PlayerPoints").GetComponentInChildren<Text>();
        pInv = player1.GetComponent<PlayerInventory>();
        player1.GetComponent<PlayerMovement>().enabled = false;
        player1.transform.GetChild(0).gameObject.SetActive(false);

        invMan = GetComponent<InventoryManager>();
    }

    void Start(){
        InitializeItemB();
        pointsLeft = pStats.PlayerPoints;
        pointText.text = "Points Left: " + pointsLeft;
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
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{pInv.AddItem(item, UpdatePoints(item.costM, item));});
                iOpt.GetComponentInChildren<Text>().text = item.itemName;

                //iOpt.GetComponentInChildren<Image>() = item.image; // IMPLEMENT WHEN ITEM OBJECT CONTAIN IMAGE REFERENCE
                index++;
            }
        }
        else{
            Debug.Log("itemOption prefab was not set");
        }
        
    }

    private bool UpdatePoints(int itemCost, Item item){
        if(pInv.GetItems().Contains(item)){
            pointsLeft += itemCost;
            pointText.text = "Points Left: " + pointsLeft;
            return true;
        }
        else if(!pInv.GetItems().Contains(item) && (pointsLeft - itemCost) >= 0){
            pointsLeft -= itemCost;
            pointText.text = "Points Left: " + pointsLeft;
            return true;
        }
        else{
            return false;
        }
    }

    public void nextScene(){
        player1.GetComponent<PlayerMovement>().enabled = true;
        player1.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

}
