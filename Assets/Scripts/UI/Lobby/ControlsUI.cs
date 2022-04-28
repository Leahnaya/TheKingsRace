using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
