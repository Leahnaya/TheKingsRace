using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CoolDown : MonoBehaviour
{
    //used to check if the playr has abillities
    public PlayerStats stats;
    public GameObject uiPrebab;
    public SpecialItem dashItem;
    public SpecialItem nitroItem;
    // Start is called before the first frame update
    void Start(){
        if(stats.HasNitro == true){
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(this.gameObject.transform);
            temp.name = nitroItem.name;
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.GetComponent<UICoolDown>().setCoolDownTime(nitroItem.cooldownM);
            //set position on canvas
            //due to low number, gonna hardcode this to be first
            temp.transform.localPosition = new Vector3(-850, 425);
            //set icon to ui icon
            temp.transform.GetComponent<Image>().sprite = nitroItem.itemSprite;
        }
        if(stats.HasDash == true){
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(this.gameObject.transform);
            temp.name = dashItem.name;
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.GetComponent<UICoolDown>().setCoolDownTime(dashItem.cooldownM);

            //set image 
            temp.transform.GetComponent<Image>().sprite = dashItem.itemSprite;

            //if has nitro, make it below nitro icon
            if (stats.HasNitro == true)
            {
                temp.transform.localPosition = new Vector3(-850, 225);
            }
            //if not, make this icon top of screen
            else{
                temp.transform.localPosition = new Vector3(-850, 425);
            }
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
