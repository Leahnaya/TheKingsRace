using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoolDown : MonoBehaviour
{
    public PlayerInventory Inv;
    private List<SpecialItem> specialItems = new List<SpecialItem>();
    public GameObject uiPrebab; 
    // Start is called before the first frame update
    void Start(){
        //retrieve all items that are special items here
        foreach(var I in Inv.PlayerItemDict){
           if(I.Value.GetType() == typeof(SpecialItem)){
                SpecialItem temp = (SpecialItem)I.Value;
                //if not
                if(temp.cooldownM == 0 || I.Value.GetType() != typeof(SpecialItem)){
                   Debug.Log("Item not fully intialized");
                   //throw assert
                   Debug.Assert(temp.cooldownM != 0);
                   Debug.Assert(I.Value.GetType() == typeof(SpecialItem));

                }
                specialItems.Add(temp);
            }
        }
      //set up ui elements for special items 
      for(int i = 0; i < specialItems.Count; i++){
            //instantiate prefab
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(this.gameObject.transform);
            SpecialItem currentSpecialItem = specialItems[i];
            temp.name = currentSpecialItem.itemName;
            //set position
            temp.transform.localPosition = new Vector3(-290, 110 - (i * 50), temp.transform.position.z);
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //set cooldown on ui
            temp.GetComponent<UICoolDown>().setCoolDownTime(currentSpecialItem.cooldownM);
        }
    }

    public void startUICooldown(string name){
        this.gameObject.transform.Find(name).GetComponent<UICoolDown>().startCoolDown();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
