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
