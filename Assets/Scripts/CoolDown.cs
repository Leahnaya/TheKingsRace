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
    public Sprite slide;
    public Sprite spaceBarIcon;
    public Item grapple;
    public Item Glide;
    public GameObject boxHighlight;
    // Start is called before the first frame update
    void Start() { 

    }
    public void startUICooldown(string name)
    {
        boxHighlight.transform.Find(name).GetComponent<UICoolDown>().startCoolDown();
    }
    // Update is called once per frame
    void Update()
    {
        ////if tap is pressed, set active reversed 
        //not currently bounded to controller
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (boxHighlight.GetComponent<Image>().enabled == false)
            {
                ////due to setup, I will have to manually turn the other ones back on so cooldown works
                //try turning on nitro
                for (int i = 0; i < boxHighlight.transform.childCount; i++)
                {
                    Transform temp = boxHighlight.transform.GetChild(i);
                    //turn off all but the special items
                    if (!temp.transform.name.Equals(nitroItem.name) && !temp.transform.gameObject.name.Equals(dashItem.name))
                    {
                        temp.gameObject.SetActive(true);
                    }
                }
                boxHighlight.GetComponent<Image>().enabled = true;
                if (stats.HasNitro == true)
                {
                    boxHighlight.transform.Find(nitroItem.name).GetComponent<Image>().enabled = true;
                    boxHighlight.transform.Find(nitroItem.name).GetChild(0).gameObject.SetActive(true);
                }
                if (stats.HasDash == true)
                {
                    boxHighlight.transform.Find(dashItem.name).GetComponent<Image>().enabled = true;
                    boxHighlight.transform.Find(dashItem.name).GetChild(0).gameObject.SetActive(true);
                }

                //turn on highlights and its children
            }
            else
            {
                for (int i = 0; i < boxHighlight.transform.childCount; i++)
                {
                    Transform temp = boxHighlight.transform.GetChild(i);
                    //turn off all but the special items
                    if (!temp.transform.name.Equals(nitroItem.name) && !temp.transform.gameObject.name.Equals(dashItem.name))
                    {
                        temp.gameObject.SetActive(false);
                    }
                }
                boxHighlight.GetComponent<Image>().enabled = false;

                if (stats.HasNitro == true)
                {
                    boxHighlight.transform.Find(nitroItem.name).GetComponent<Image>().enabled = false;
                    boxHighlight.transform.Find(nitroItem.name).GetChild(0).gameObject.SetActive(false);
                }
                if (stats.HasDash == true)
                {
                    boxHighlight.transform.Find(dashItem.name).GetComponent<Image>().enabled = false;
                    boxHighlight.transform.Find(dashItem.name).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    public void populatePlayerCanvas()
    {
        ////display guranteed items
        // temp pos var
        float posTemp = 0f;
        int itemsAdded = 0;

        //kick
        GameObject temp3 = Instantiate(uiPrebab);
        //set parent as highlight
        temp3.transform.SetParent(boxHighlight.transform);
        temp3.name = kickItem.name;
        temp3.transform.localRotation = Quaternion.identity;
        //due to low number, gonna hardcode this to be first
        temp3.transform.localPosition = new Vector3(-100, -40);
        temp3.transform.localScale = new Vector3(.75f,.75f,.75f);
        //set icon to ui icon

        if (GameManager.GM.bindableActions["kickKey"] == KeyCode.LeftShift)
        {
            temp3.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
        }
        //else set to regular size
        else
        {
            temp3.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        }

        temp3.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["kickKey"]];
        temp3.transform.GetComponent<Image>().sprite = kickItem.itemSprite;


        //slide
        GameObject temp4 = Instantiate(uiPrebab);
        //set parent as highlight
        temp4.transform.SetParent(boxHighlight.transform);
        temp4.name = "slide";
        temp4.transform.localRotation = Quaternion.identity;
        //due to low number, gonna hardcode this to be first
        temp4.transform.localPosition = new Vector3(-260, -40);
        temp4.transform.localScale = new Vector3(.75f,.75f,.75f);

        if (GameManager.GM.bindableActions["slideKey"] == KeyCode.LeftShift)
        {
            temp4.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
        }
        //else set to regular size
        else
        {
            temp4.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        }

        temp4.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["slideKey"]];
        temp4.transform.GetComponent<Image>().sprite = slide;
        //set icon to ui icon
        //doesn't exists


        ////check if items are there
        //grapple

        //apply special items (can condense and simplify if needed)
        if (stats.HasNitro == true)
        {
            GameObject temp = Instantiate(uiPrebab);
            temp.transform.SetParent(boxHighlight.transform);
            temp.name = nitroItem.name;
            temp.transform.localRotation = Quaternion.identity;
            temp.transform.localRotation.Set(0f, 0f, 0f, 0f);
            temp.transform.localScale = new Vector3(.75f,.75f,.75f);
            temp.GetComponent<UICoolDown>().setCoolDownTime(nitroItem.cooldownM);
            //set position on canvas
            //due to low number, gonna hardcode this to be first

            //increment items
            if (itemsAdded == 0)
            {
                temp.transform.localPosition = new Vector3(220, 80);
                itemsAdded++;
            }
            //increment items
            else if (itemsAdded >= 1)
            {
                temp.transform.localPosition = new Vector3(60 + posTemp, 80);
                posTemp -= 160;
                itemsAdded++;
            }

            if (GameManager.GM.bindableActions["nitroKey"] == KeyCode.LeftShift)
            {
                temp.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
            }
            //else set to regular size
            else
            {
                temp.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            }

            //set icon to ui icon
            temp.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["nitroKey"]];
            temp.transform.GetComponent<Image>().sprite = nitroItem.itemSprite;
            //set button control (hard coded)
        }
        if (stats.HasDash == true)
        {
            GameObject temp2 = Instantiate(uiPrebab);
            temp2.transform.SetParent(boxHighlight.transform);
            temp2.name = dashItem.name;
            temp2.transform.localRotation = Quaternion.identity;
            temp2.transform.localScale = new Vector3(.75f,.75f,.75f);
            //increment items
            if (itemsAdded == 0)
            {
                temp2.transform.localPosition = new Vector3(220, 80);
                itemsAdded++;
            }
            //increment items
            else if (itemsAdded >= 1)
            {
                temp2.transform.localPosition = new Vector3(60 + posTemp, 80);
                posTemp -= 160;
                itemsAdded++;
            }
            temp2.GetComponent<UICoolDown>().setCoolDownTime(dashItem.cooldownM);
            //set image 

            if (GameManager.GM.bindableActions["dashKey"] == KeyCode.LeftShift)
            {
                temp2.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
            }
            //else set to regular size
            else
            {
                temp2.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            }


            temp2.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["dashKey"]];
            temp2.transform.GetComponent<Image>().sprite = dashItem.itemSprite;
            //set button control


        }
        if (stats.HasGrapple == true)
        {
            GameObject temp5 = Instantiate(uiPrebab);
            //set parent as highlight
            temp5.transform.SetParent(boxHighlight.transform);
            temp5.name = grapple.itemName;
            temp5.transform.localRotation = Quaternion.identity;
            temp5.transform.localScale = new Vector3(.75f,.75f,.75f);
            //increment items
            if (itemsAdded == 0)
            {
                temp5.transform.localPosition = new Vector3(220, 80);
                itemsAdded++;
            }
            //increment items
            else if (itemsAdded >= 1)
            {
                temp5.transform.localPosition = new Vector3(60 + posTemp, 80);
                posTemp -= 160;
                itemsAdded++;
            }
            //set icon to ui icon
            if (GameManager.GM.bindableActions["grappleKey"] == KeyCode.LeftShift)
            {
                temp5.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
            }
            //else set to regular size
            else
            {
                temp5.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            }


            temp5.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["grappleKey"]];
            temp5.transform.GetComponent<Image>().sprite = grapple.itemSprite;
            //doesn't exists
            //increment counter
        }
        //glider
        if (stats.HasGlider == true)
        {
            GameObject temp6 = Instantiate(uiPrebab);
            //set parent as highlight
            temp6.transform.SetParent(boxHighlight.transform);
            temp6.name = Glide.itemName;
            temp6.transform.localRotation = Quaternion.identity;
            temp6.transform.localScale = new Vector3(.75f,.75f,.75f);
            //increment items
            if (itemsAdded == 0)
            {
                temp6.transform.localPosition = new Vector3(220, 80);
                itemsAdded++;
            }
            //increment items
            else if (itemsAdded >= 1)
            {
                temp6.transform.localPosition = new Vector3(60 + posTemp, 80);
                posTemp -= 160;
                itemsAdded++;
            }
            //set icon to ui icon
            temp6.transform.Find("keyBoardIcon").GetComponent<Image>().sprite = spaceBarIcon;
            temp6.transform.Find("keyBoardIcon").GetComponent<RectTransform>().sizeDelta = new Vector2(145, 100);
            temp6.transform.GetComponent<Image>().sprite = Glide.itemSprite;
            //doesn't exists
            //increment counter
        }

    }
}

