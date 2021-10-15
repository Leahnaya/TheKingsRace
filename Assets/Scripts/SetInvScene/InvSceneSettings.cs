using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvSceneSettings : MonoBehaviour
{
    GameObject player1;
    PlayerInventory pInv;
    public GameObject itemOptPrefab;

    InventoryManager invMan; 

    // Start is called before the first frame update
    void Awake(){
        player1 = GameObject.Find("PlayerPrefab");
        pInv = player1.GetComponent<PlayerInventory>();
        player1.GetComponent<PlayerMovement>().enabled = false;
        player1.transform.GetChild(0).gameObject.SetActive(false);

        invMan = GetComponent<InventoryManager>();
    }

    void Start(){
        InitializeItemB();
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void InitializeItemB(){
        int index = 0;
        if(itemOptPrefab != null){
            foreach(Item item in invMan.ItemList){

                Vector3 position = new Vector3((1+(index)),3,0);
                var iOpt = Instantiate(itemOptPrefab, position, Quaternion.identity);

                iOpt.name = item.itemName;
                iOpt.transform.SetParent(GameObject.Find("Items").transform);
                iOpt.transform.localScale = new Vector3(1,1,1);
                iOpt.GetComponent<Button>().onClick.AddListener(delegate{pInv.AddItem(item);});
                iOpt.GetComponentInChildren<Text>().text = item.itemName;

                //iOpt.GetComponentInChildren<Image>() = item.image; // IMPLEMENT WHEN ITEM OBJECT CONTAIN IMAGE REFERENCE
                index++;
            }
        }
        else{
            Debug.Log("itemOption prefab was not set");
        }
        
    }

}
