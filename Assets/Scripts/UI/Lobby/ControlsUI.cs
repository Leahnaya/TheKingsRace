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
    public GameObject kingControls;



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
            ControlsHeader.text = "King Controls";
            ControlsButtonText.text = "Runner Controls";

            runnerControls.SetActive(false);
            kingControls.SetActive(true);
        }
        else
        {
            ControlsHeader.text = "Runner Controls";
            ControlsButtonText.text = "King Controls";

            runnerControls.SetActive(true);
            kingControls.SetActive(false);
        }
    }
}
