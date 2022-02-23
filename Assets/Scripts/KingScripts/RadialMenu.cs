using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{

    bool UIOn = false;
    public GameObject King;
    // Update is called once per frame
    void Update()
    {
        if (UIOn == false) {
            transform.position = Input.mousePosition;//Moved the UI to where the mouse is
        }
        if(Input.GetAxis("RadialMenu") > 0) {//Pressing Q makes the UI appear
            if (King.GetComponent<KingPlace>().FirstPlacing == true) {
                King.GetComponent<KingPlace>().CancelPlacing();
            }
            MenuOn();
        }
        if (Input.GetAxis("RadialMenu") < 0) {//Pressing E makes the UI dissapear
            MenuOff();
        }
    }

    private void MenuOn()
    {
        foreach (Transform child in transform) { //Turns on all of the different buttons attached to this panel and stops it from moving
            child.gameObject.SetActive(true);
            UIOn = true;
        }
    }

    public void MenuOff()
    {
        foreach (Transform child in transform) { //Turns off all of the different buttons attached to this panel and starts it moving again
            child.gameObject.SetActive(false);
            UIOn = false;
        }
    }
}
