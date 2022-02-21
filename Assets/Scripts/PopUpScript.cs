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

    //for some reason on enter was triggering twice (shouldn't happen but couldn't figure out why
    private bool isPop = false;
    private GameObject temp;
    private GameObject canvas;
    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ArcherTarget" && isPop == false)
        {
            Debug.Log("enter");
            //if the item is not set or values are null, catch it
            try
            {

                temp = Instantiate(uiPrebab);
                //set parent as highlight
                temp.transform.SetParent(canvas.transform);
                temp.name = item.name;
                temp.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                //due to low number, gonna hardcode this to be first
                temp.transform.localPosition = new Vector3(0, 0);
                //set icon to ui icon
                temp.transform.GetComponent<Image>().sprite = item.itemSprite;
                //set text
                temp.transform.Find("Prompt").GetComponent<TMPro.TextMeshProUGUI>().text = message;
                //no longer allow instnatiation
                isPop = true;
                Debug.Log("create");
            }
            catch (System.Exception e)
            {
                Debug.Log("error with item");
            }

        }


    }
    private void OnTriggerExit(Collider other)
    {
        //delete ui element when exit
        if (other.gameObject.tag == "ArcherTarget")
        {
            Debug.Log("exit");
            Destroy(temp);
            isPop = false;
        }
    }

}
