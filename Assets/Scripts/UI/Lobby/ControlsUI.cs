using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlsUI : MonoBehaviour
{
    public GameObject controlsMenu;
    public TMP_Text ControlsHeader;
    public TMP_Text ControlsButtonText;
    public TMP_Text ControlsText;
    private bool isViewingRunnerControls = true;



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
        if (isViewingRunnerControls)
        {
            isViewingRunnerControls = false;
            ControlsHeader.text = "King Controls";
            ControlsButtonText.text = "Runner Controls";
            ControlsText.text = "To-Do: Put the King's Controls here";
        }
        else
        {
            isViewingRunnerControls = true;
            ControlsHeader.text = "Runner Controls";
            ControlsButtonText.text = "King Controls";
            ControlsText.text = "To-Do: Put the Runner's Controls here";
        }
    }
}
