using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenuScript : MonoBehaviour
{
    public GameObject keyboardControlsMenu;
    public GameObject gamePadControlsMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if vertical input from right analog
        if (Input.GetAxis("KingVerticalMouseMove") != 0 || Input.GetAxis("KingHorizontalMouseMove") != 0)
        {
            //if off, turn it on
            if(gamePadControlsMenu.activeInHierarchy == false)
            {
                keyboardControlsMenu.SetActive(false);
                gamePadControlsMenu.SetActive(true);
            }
        }
        //check if mouse movement if so (turn off pointer)
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (keyboardControlsMenu.activeInHierarchy == false)
            {
                keyboardControlsMenu.SetActive(true);
                gamePadControlsMenu.SetActive(false);
            }
        }
    }
}
