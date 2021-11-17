using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    //List of items
    [SerializeField] List<Item> items;

    //Scripts
    public PlayerStats pStats;
    
    void Awake(){
        pStats = GetComponent<PlayerStats>();
        DontDestroyOnLoad(transform.parent.gameObject);
        
    }

    //Add item to player Inventory
    public void AddItem(Item item, bool ableToAdd){
        if(!items.Contains(item) && ableToAdd){
            Debug.Log("Item Added");

            //Add item to player inventroy list
            items.Add(item);
            //Equip item
            item.Equip(pStats, this.gameObject);
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
