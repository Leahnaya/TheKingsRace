using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsUI : MonoBehaviour
{
    public GameObject controlsMenu;
    public TMP_Text ControlsHeader;
    public TMP_Text ControlsButtonText;

    public GameObject runnerControls;
    public GameObject keyboardRunnerControlsMenu;
    public GameObject gamePadRunnerControlsMenu;

    public GameObject kingControls;
    public GameObject keyboardKingControlsMenu;
    public GameObject gamePadKingControlsMenu;
    private bool isKing = false;

    public GameObject slideKey;
    public GameObject kickKey;
    public GameObject grappleKey;
    public GameObject dashKey;
    public GameObject gliderKey;
    public GameObject nitroKey;

    void Start(){
        UpdateKeyImages();
    }

    // Update is called once per frame
    void Update()
    {
        //if vertical input from right analog
        if (Input.GetAxis("KingVerticalMouseMove") != 0 || Input.GetAxis("KingHorizontalMouseMove") != 0)
        {
            if(isKing){
                            //if off, turn it on
                if(gamePadKingControlsMenu.activeInHierarchy == false)
                {
                    keyboardKingControlsMenu.SetActive(false);
                    gamePadKingControlsMenu.SetActive(true);
                }
            }
            else{
                //if off, turn it on
                if(gamePadRunnerControlsMenu.activeInHierarchy == false)
                {
                    keyboardRunnerControlsMenu.SetActive(false);
                    gamePadRunnerControlsMenu.SetActive(true);
                }
            }

        }
        //check if mouse movement if so (turn off pointer)
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if(isKing){
                //if off, turn it on
                if(keyboardKingControlsMenu.activeInHierarchy == false)
                {
                    keyboardKingControlsMenu.SetActive(true);
                    gamePadKingControlsMenu.SetActive(false);
                }
            }
            else{
                //if off, turn it on
                if(keyboardRunnerControlsMenu.activeInHierarchy == false)
                {
                    keyboardRunnerControlsMenu.SetActive(true);
                    gamePadRunnerControlsMenu.SetActive(false);
                }
            }
        }
    }

    public void toggleMenu()
    {
        //if active, turn off
        if (controlsMenu.activeInHierarchy)
        {
            controlsMenu.SetActive(false);
        }
        //if off, turn on
        else
        {
            controlsMenu.SetActive(true);
        }
    }

    public void ToggleControlsScreen()
    {
        if (runnerControls.active)
        {
            isKing = true;
            ControlsHeader.text = "King Controls";
            ControlsButtonText.text = "Runner Controls";

            runnerControls.SetActive(false);
            kingControls.SetActive(true);
        }
        else
        {
            isKing = false;
            ControlsHeader.text = "Runner Controls";
            ControlsButtonText.text = "King Controls";

            runnerControls.SetActive(true);
            kingControls.SetActive(false);
        }
    }

    private void UpdateKeyImages(){
        //Slide
        if (GameManager.GM.bindableActions["slideKey"] == KeyCode.LeftShift)
        {
            slideKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 60);
        }
        //else set to regular size
        else
        {
            slideKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        }
        slideKey.transform.GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["slideKey"]];

        //Kick
        if (GameManager.GM.bindableActions["kickKey"] == KeyCode.LeftShift)
        {
            kickKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 60);
        }
        //else set to regular size
        else
        {
            kickKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        }
        kickKey.transform.GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["kickKey"]];

        //Nitro
        if (GameManager.GM.bindableActions["nitroKey"] == KeyCode.LeftShift)
        {
            nitroKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 50);
            nitroKey.transform.localPosition = new Vector3(-55, -40, 0);
        }
        //else set to regular size
        else
        {
            nitroKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            nitroKey.transform.localPosition = new Vector3(-20, -35, 0);
        }
        nitroKey.transform.GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["nitroKey"]];

        //Dash
        if (GameManager.GM.bindableActions["dashKey"] == KeyCode.LeftShift)
        {
            dashKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 50);
            dashKey.transform.localPosition = new Vector3(-55, -40, 0);
        }
        //else set to regular size
        else
        {
            dashKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            dashKey.transform.localPosition = new Vector3(-20, -35, 0);
        }
        dashKey.transform.GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["dashKey"]];

        //Grapple
        if (GameManager.GM.bindableActions["grappleKey"] == KeyCode.LeftShift)
        {
            grappleKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 50);
            grappleKey.transform.localPosition = new Vector3(-55, -40, 0);
        }
        //else set to regular size
        else
        {
            grappleKey.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            grappleKey.transform.localPosition = new Vector3(-20, -35, 0);
        }
        grappleKey.transform.GetComponent<Image>().sprite = GameManager.GM.keyToSpriteDict[GameManager.GM.bindableActions["grappleKey"]];

    }
}
