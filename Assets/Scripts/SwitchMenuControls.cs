using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenuControls : MonoBehaviour
{
    public GameObject currentMenu;
    public GameObject nextMenu;
    // Update is called once per frame
    public void enable()
    {
        nextMenu.SetActive(true);
        currentMenu.SetActive(false);

    }
}
