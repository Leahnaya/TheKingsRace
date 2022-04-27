using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMenuControls : MonoBehaviour
{
    public GameObject playerMenu;
    public GameObject kingMenu;
    // Update is called once per frame
    public void enable()
    {
        if(playerMenu.active){
            kingMenu.SetActive(true);
            playerMenu.SetActive(false);    
        }
        else{
            kingMenu.SetActive(false);
            playerMenu.SetActive(true);    
        }

    }
}
