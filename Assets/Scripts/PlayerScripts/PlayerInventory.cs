using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{

    //List of items
    [SerializeField] List<Item> items;

    //Scripts
    public PlayerStats pStats;
    
    void Awake(){
        pStats = GetComponent<PlayerStats>();     
    }
    
    public void UpdateInventory(Item item, bool ableToAdd){
        if(!items.Contains(item) && ableToAdd){
            Debug.Log("Item Added");
            AddItem(item);
        }
        else if(!ableToAdd && !items.Contains(item)){
            //Item cannot be added
            Debug.Log("Item Cannot Be Added");
        }
        else{
            //If player already has the item it removes it
            Debug.Log("Item Removed");
            RemoveItem(item);
        }
    }

    public void UpdateEquips(){
        foreach(Item item in items){
            item.Equip(pStats, this.gameObject);
        }
    }

    //Add item to player Inventory
    public void AddItem(Item item){

        //Adds and equips Item if it can
        items.Add(item);

        item.Equip(pStats, this.gameObject);
        
    }

    //Add item to player Inventory
    public void AddItemNetwork(Item item){

        //Adds and equips Item if it can
        items.Add(item);
        
    }

    //Remove Item player inv
    public void RemoveItem(Item item){
        if(items.Remove(item)){
            item.Unequip(pStats, this.gameObject);
        }
    }

    //GetItems
    public List<Item> GetItems(){
        return items;
    }


}
