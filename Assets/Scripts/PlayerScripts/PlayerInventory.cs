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
        DontDestroyOnLoad(this.gameObject);
        
    }

    public void AddItem(Item item, bool ableToAdd){
        if(!items.Contains(item) && ableToAdd){
            Debug.Log("Item Added");
            items.Add(item);
            item.Equip(pStats);
        }
        else if(!ableToAdd && !items.Contains(item)){
            Debug.Log("Item Cannot Be Added");
        }
        else{
            Debug.Log("Item Removed");
            RemoveItem(item);
        }
        
    }

    public void AddSpecialItem<T>(T itemCandidate) { // Unless we don't want the four special items to be handled by inventory/inventory manager?
        if (itemCandidate is Item) {
            items.Add(itemCandidate as Item);
        }
    }


    public void RemoveItem(Item item){
        if(items.Remove(item)){
            item.Unequip(pStats);
        }
    }

    public List<Item> GetItems(){
        return items;
    }


}
