using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//this is intended to be attached to a canvasd
public class CoolDown : MonoBehaviour
{
    //used to check if the playr has abillities
    public PlayerStats stats;
    public GameObject uiPrebab;
    //special items
    public SpecialItem dashItem;
    public SpecialItem nitroItem;
    //regular items with buttons
    public Item kickItem;
    public Item grapple;
    public Item Glide;
    public GameObject boxHighlight;
    // Start is called before the first frame update
    void Start(){

      
      



    }
    public void startUICooldown(string name){
        this.gameObject.transform.Find(name).GetComponent<UICoolDown>().startCoolDown();
    }
    // Update is called once per frame
    void Update(){
        ////if tap is pressed, set active reversed 
        //not currently bounded to controller
        if (Input.GetKeyDown(KeyCode.Tab)){
            if(boxHighlight.activeInHierarchy == false){
                boxHighlight.SetActive(true);
            }
            else{
                boxHighlight.SetActive(false);
            }
        }
    }
    public void populatePlayerCanvas()
    {
        //apply special items (can condense and simplify if needed)
        if (stats.HasNitro == true)
        {
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(this.gameObject.transform);
            temp.name = nitroItem.name;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.transform.localRotation.Set(0f, 0f, 0f, 0f);
            temp.GetComponent<UICoolDown>().setCoolDownTime(nitroItem.cooldownM);
            //set position on canvas
            //due to low number, gonna hardcode this to be first
            temp.transform.localPosition = new Vector3(-850, 425);
            //set icon to ui icon
            temp.transform.GetComponent<Image>().sprite = nitroItem.itemSprite;
            //set button control (hard coded)
            temp.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Shift";
        }
        if (stats.HasDash == true)
        {
            GameObject temp2 = Instantiate(uiPrebab);
            temp2.transform.SetParent(this.gameObject.transform);
            temp2.name = dashItem.name;
            temp2.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp2.transform.localRotation = Quaternion.identity;
            temp2.GetComponent<UICoolDown>().setCoolDownTime(dashItem.cooldownM);

            //set image 
            temp2.transform.GetComponent<Image>().sprite = dashItem.itemSprite;

            //if has nitro, make it below nitro icon
            if (stats.HasNitro == true)
            {
                temp2.transform.localPosition = new Vector3(-850, 225);
            }
            //if not, make this icon top of screen
            else
            {
                temp2.transform.localPosition = new Vector3(-850, 425);
            }
            //set button control
            temp2.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "R";
        }

        ////display guranteed items
        // temp pos var
        float posTemp = 0f;
        //kick
        GameObject temp3 = Instantiate(uiPrebab);
        //set parent as highlight
        temp3.transform.SetParent(boxHighlight.transform);
        temp3.name = kickItem.name;
        temp3.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        temp3.transform.localRotation = Quaternion.identity;
        //due to low number, gonna hardcode this to be first
        temp3.transform.localPosition = new Vector3(-100, -100);
        //set icon to ui icon
        temp3.transform.GetComponent<Image>().sprite = kickItem.itemSprite;
        temp3.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "F";

        //slide
        GameObject temp4 = Instantiate(uiPrebab);
        //set parent as highlight
        temp4.transform.SetParent(boxHighlight.transform);
        temp4.name = "slide";
        temp4.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        temp4.transform.localRotation = Quaternion.identity;
        //due to low number, gonna hardcode this to be first
        temp4.transform.localPosition = new Vector3(100, -100);
        //set icon to ui icon
        //doesn't exists
        temp4.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Slide \n Q";

        ////check if items are there
        //grapple
        if (stats.HasGrapple == true)
        {
            GameObject temp5 = Instantiate(uiPrebab);
            //set parent as highlight
            temp5.transform.SetParent(boxHighlight.transform);
            temp5.name = grapple.itemName;
            temp5.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp5.transform.localRotation = Quaternion.identity;
            //due to low number, gonna hardcode this to be first
            temp5.transform.localPosition = new Vector3(100 + posTemp, 100);
            //set icon to ui icon
            temp5.transform.GetComponent<Image>().sprite = grapple.itemSprite;
            //doesn't exists
            temp5.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "E";
            //increment counter
            posTemp -= 200;
        }
        //glider
        if (stats.HasGlider == true)
        {
            GameObject temp6 = Instantiate(uiPrebab);
            //set parent as highlight
            temp6.transform.SetParent(boxHighlight.transform);
            temp6.name = Glide.itemName;
            temp6.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp6.transform.localRotation = Quaternion.identity;
            //due to low number, gonna hardcode this to be first
            temp6.transform.localPosition = new Vector3(100 + posTemp, 100);
            //set icon to ui icon
            temp6.transform.GetComponent<Image>().sprite = Glide.itemSprite;
            //doesn't exists
            temp6.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Hold Space";
            //increment counter
            posTemp -= 200;
        }
    }
}
