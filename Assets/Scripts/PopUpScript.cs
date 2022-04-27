using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpScript : MonoBehaviour
{
    //item that displays on pop up
    public Item item;
    public GameObject uiPrebab;
    public string message;
    public Sprite slideSprite;

    //for some reason on enter was triggering twice (shouldn't happen but couldn't figure out why
    private bool isPop = false;
    private GameObject temp;
    private Canvas canvas;

    private void OnTriggerEnter(Collider other)
    {
        //if it is the player object, no pop up is active, and they have the required item
        if (other.gameObject.tag == "ArcherTarget" && isPop == false)
        {
            //Debug.Log("enter");

            //for for cleaner code if this check is done in player or a function in this class
            //if the item is not null (item given) check if player has the item and if they do send popup
            if(item != null)
            {
                if ((other.transform.GetComponent<PlayerStats>().HasDash && item.name == "Dash") || (other.transform.GetComponent<PlayerStats>().HasGrapple && item.name == "Grapple") || (other.transform.GetComponent<PlayerStats>().HasWallrun && item.name == "Wall Run") || (other.transform.GetComponent<PlayerStats>().HasGlider && item.name == "Glider"))
                {
                    setUpItemPopUp(other);
                }
            }
            // if item is not set, it assumed that the popup is used for slidding (no item for slidding exists so this
            // is necessary)
            else if(item == null)
            {
                setUpSlidePopUp(other);
            }
           

        }


    }
    private void OnTriggerExit(Collider other)
    {
        //delete ui element when exit if it exist
        if (other.gameObject.tag == "ArcherTarget" && temp != null)
        {
            //Debug.Log("exit");
            //remove reference to canvas
            canvas = null;
            //attempt to find 
            Destroy(temp);
            isPop = false;
        }
    }

    private void setUpItemPopUp(Collider other)
    {

        //if the item is not set or values are null, catch it
        try
        {

            //try to get canvas from player
            canvas = other.transform.root.GetComponentInChildren<Canvas>();
            temp = Instantiate(uiPrebab);
            //set parent as highlight
            temp.transform.SetParent(canvas.transform);
            temp.name = item.name + "PopUp";
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //due to low number, gonna hardcode this to be first
            temp.transform.localPosition = new Vector3(-250, -430);
            temp.transform.localRotation = Quaternion.identity;
            //set icon to ui icon
            temp.transform.GetComponent<Image>().sprite = item.itemSprite;
            //set text
            temp.transform.Find("Prompt").GetComponent<TMPro.TextMeshProUGUI>().text = message;
            //no longer allow instnatiation
            isPop = true;
            //Debug.Log("create");
        }
        catch (System.Exception e)
        {
            Debug.Log("error with item");
        }

    }

    private void setUpSlidePopUp(Collider other)
    {

        //if the item is not set or values are null, catch it
        try
        {

            //try to get canvas from player
            canvas = other.transform.root.GetComponentInChildren<Canvas>();
            temp = Instantiate(uiPrebab);
            //set parent as highlight
            temp.transform.SetParent(canvas.transform);
            temp.name = "SlidePopUp";
            temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            temp.transform.localRotation = Quaternion.identity;
            //due to low number, gonna hardcode this to be first
            temp.transform.localPosition = new Vector3(-250, -430);
            //set icon to ui icon
            temp.transform.GetComponent<Image>().sprite = slideSprite;
            //set text
            temp.transform.Find("Prompt").GetComponent<TMPro.TextMeshProUGUI>().text = message;
            //no longer allow instnatiation
            isPop = true;
            //Debug.Log("create");
        }
        catch (System.Exception e)
        {
            Debug.Log("error with item");
        }

    }
}
