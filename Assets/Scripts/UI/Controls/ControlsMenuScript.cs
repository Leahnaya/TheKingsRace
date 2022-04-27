using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlsMenuScript : MonoBehaviour
{
    public GameObject keyboardRunnerControlsMenu;
    public GameObject gamePadRunnerControlsMenu;
    public GameObject keyboardKingControlsMenu;
    public GameObject gamePadKingControlsMenu;

    public TextMeshProUGUI buttonText;
    private bool isKing = false;

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
            if(isKing){
                            //if off, turn it on
                if(gamePadKingControlsMenu.activeInHierarchy == false)
                {
                    keyboardKingControlsMenu.SetActive(false);
                    gamePadKingControlsMenu.SetActive(true);
                }
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
                if(gamePadKingControlsMenu.activeInHierarchy == false)
                {
                    keyboardKingControlsMenu.SetActive(true);
                    gamePadKingControlsMenu.SetActive(false);
                }
                //if off, turn it on
                if(gamePadKingControlsMenu.activeInHierarchy == false)
                {
                    keyboardKingControlsMenu.SetActive(true);
                    gamePadKingControlsMenu.SetActive(false);
                }
            }
            else{
                //if off, turn it on
                if(gamePadRunnerControlsMenu.activeInHierarchy == false)
                {
                    keyboardRunnerControlsMenu.SetActive(true);
                    gamePadRunnerControlsMenu.SetActive(false);
                }
                //if off, turn it on
                if(gamePadRunnerControlsMenu.activeInHierarchy == false)
                {
                    keyboardRunnerControlsMenu.SetActive(true);
                    gamePadRunnerControlsMenu.SetActive(false);
                } 
            }
        }
    }

    public void SwapButtonText(){
        if(!isKing){
            buttonText.text = "Runner Controls";
            isKing = true;
        }
        else{
            buttonText.text = "King Controls";
            isKing = false;
        }
    }
}
