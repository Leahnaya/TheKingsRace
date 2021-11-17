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
      

      void Awake(){
            //Gets Items in Resource Folder
            allItems = Resources.LoadAll<Item>("ItemObjects");
            itemList = new List<Item>(allItems);
      }
}
