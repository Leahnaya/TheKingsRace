using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  public Item[] allItems;
  private List<Item> itemList;
  public List<Item> ItemList{
        get{ return itemList; }
        set{ itemList = value; }
  }
  void Awake(){
        allItems = Resources.LoadAll<Item>("ItemObjects");
        
        itemList = new List<Item>(allItems);
  }
}
