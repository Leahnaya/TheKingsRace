using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{

    //List of items
    [SerializeField] private Dictionary<string, Item> playerItemDict = new Dictionary<string, Item>();
    public Dictionary<string, Item> PlayerItemDict{
        get{ return playerItemDict; }
    }

    [SerializeField] private List<string> networkItemList = new List<string>();
    public List<string> NetworkItemList{
        get{ return networkItemList; }
    }
    //Scripts
    public PlayerStats pStats;
    public InventoryManager invMan;
    
    void Awake(){
        pStats = GetComponent<PlayerStats>();
        invMan =  FindObjectOfType<InventoryManager>();   
    }
    
    //Adds player Item to a dictionary
    void AddItem(Item item){
        playerItemDict.Add(item.name, item);
        item.Equip(pStats, this.gameObject);
    }

     //Add item to player Inventory
    public void UpdateItemNetwork(string itemName, int add){

        if(add == 0) networkItemList.Remove(itemName);

        else if(add == 1 )networkItemList.Add(itemName);

        else Debug.Log("Nothing updated");
    }

    //Remove Item player inv
    public void RemoveItem(Item item){
        if(playerItemDict.Remove(item.name)){
            item.Unequip(pStats, this.gameObject);
        }
    }

    //Updates Inventory to Add item to player list
    public int UpdateInventory(Item item, bool ableToAdd){
        if(!playerItemDict.ContainsKey(item.name) && ableToAdd){
            Debug.Log("Item Added");
            AddItem(item);
            return 1;
        }
        else if(!ableToAdd && !playerItemDict.ContainsKey(item.name)){
            //Item cannot be added
            Debug.Log("Item Cannot Be Added");
            return -1;
        }
        else{
            //If player already has the item it removes it
            Debug.Log("Item Removed");
            RemoveItem(item);
            return 0;
        }
    }

    //Equips All items in list at start of next scene
    public void UpdateEquips(){
        if(invMan != null){
            foreach(string itemName in networkItemList){
                invMan.ItemDict[itemName].Equip(pStats, this.gameObject);
            }
        }
        else{
            Debug.Log("invMan is currently not set or disabled");
        }
    }


}
