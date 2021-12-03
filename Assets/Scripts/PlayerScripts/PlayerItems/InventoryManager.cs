using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
      //allItems array
      public Item[] allItems;

      //Item list
      private List<Item> itemList;
      public List<Item> ItemList{
        get{ return itemList; }
        set{ itemList = value; }
      }
      
      //Item Dictionary
      private Dictionary<string, Item> itemDict = new Dictionary<string, Item>();
      public Dictionary<string, Item> ItemDict{
            get{ return itemDict; }
      }

      void Awake(){
            //Gets Items in Resource Folder
            allItems = Resources.LoadAll<Item>("ItemObjects");
            itemList = new List<Item>(allItems);

            foreach(Item item in itemList){
                  itemDict.Add(item.name, item);
            }
      }
}
