using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    bool UIOn = false;
    public GameObject King;
    public Camera cam;

    Vector3 screenPoint;

    [SerializeField]
    private Text UItext;
    // Update is called once per frame
    void Update()
    {
        if (UIOn == false) {
            screenPoint = Input.mousePosition;
            screenPoint.z = 120f;
            transform.position = cam.ScreenToWorldPoint(screenPoint);//Moved the UI to where the mouse is
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
        UItext.text = "Press E to Close Menu";
        foreach (Transform child in transform) { //Turns on all of the different buttons attached to this panel and stops it from moving
            child.gameObject.SetActive(true);
            UIOn = true;
        }
    }

    public void MenuOff()
    {
        UItext.text = "Press Q to Open Menu";
        foreach (Transform child in transform) { //Turns off all of the different buttons attached to this panel and starts it moving again
            child.gameObject.SetActive(false);
            UIOn = false;
        }
    }

    public void PlaceObj(int ID)
    {
        switch (ID)
        {//Parses in the button clicked into the right object that the King is placing
            case 0:
                UItext.text = "Click to Place the Block";
                break;
            case 1:
                UItext.text = "Click and Hold to Resize the Hail's Area";
                break;
            case 2:
                UItext.text = "Click and Hold to Determine the Slime's Direction";
                break;
            case 3:
                UItext.text = "Click to Place the Bumper";
                break;
        }
    }
}
