using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //List of items
    [SerializeField] List<Item> items;

    //Scripts
    public PlayerStats pStats;
    public InventoryManager invMan;
    
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
        invMan = GetComponent<InventoryManager>();//////UPDATE WHEN THIS IS NO LONGER ATTACHED TO THE PLAYER
    }

    void Start(){
        AddItem(invMan.ItemList[0]);
        AddItem(invMan.ItemList[1]);
        foreach (Item item in items){
            Debug.Log(item.name);
            item.Equip(pStats);
        }
    }


}
