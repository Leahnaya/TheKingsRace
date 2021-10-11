using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] List<Item> items;
    public PlayerStats pStats;
    
    public bool AddItem(Item item){
        items.Add(item);
        return true;
    }

    public bool AddSpecialItem<T>(T itemCandidate) { // Unless we don't want the four special items to be handled by inventory/inventory manager?
        if (itemCandidate is Item) {
            items.Add(itemCandidate as Item);
        }
        return true;
    }


    public bool RemoveItem(Item item){
        if(items.Remove(item)){
            return true;
        }
        return false;
    }

    void Awake(){
        pStats = GetComponent<PlayerStats>();
        foreach (Item item in items){
            item.Equip(pStats);
        }
    }


}
