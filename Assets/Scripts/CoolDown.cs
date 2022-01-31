using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoolDown : MonoBehaviour
{
    //used to check if the playr has abillities
    public PlayerStats stats;
    public GameObject uiPrebab; 
    // Start is called before the first frame update
    void Start(){
    //check if player has abillity
        if(stats.HasNitro == true){
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(this.gameObject.transform);
            temp.name = "Nitro";
        }
          
    }
      //set up ui elements for special items 
    //   for(int i = 0; i < specialItems.Count; i++){
    //         //instantiate prefab
    //         GameObject temp = Instantiate(uiPrebab);
    //         temp.transform.SetParent(this.gameObject.transform);
    //         SpecialItem currentSpecialItem = specialItems[i];
    //         temp.name = currentSpecialItem.itemName;
    //         //set position
    //         temp.transform.localPosition = new Vector3(-290, 110 - (i * 50), temp.transform.position.z);
    //         temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    //         //set cooldown on ui
    //         temp.GetComponent<UICoolDown>().setCoolDownTime(currentSpecialItem.cooldownM);
    //     }
    //}

    public void startUICooldown(string name){
        this.gameObject.transform.Find(name).GetComponent<UICoolDown>().startCoolDown();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
