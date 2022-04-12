using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    bool UIOn = false;
    bool ButtonUp = true;
    public GameObject King;
    public Camera cam;

    Vector3 screenPoint;

    [SerializeField]
    private Text UItext;

    private float xBound = 1920f;//Resolution
    private float yBound = 1080f;

    private float xSize;
    private float ySize;

    void Start()
    {
        xSize = GetComponent<RectTransform>().sizeDelta.x / 2;//Converts the size of the Panel into the bounds of the screen
        ySize = GetComponent<RectTransform>().sizeDelta.y / 2;
        xBound -= xSize;
        yBound -= ySize;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIOn == false) {
            screenPoint = Input.mousePosition;
            screenPoint.z = 120f;
            screenPoint.x = Mathf.Clamp(screenPoint.x, xSize, xBound);//Uses the calculated bounds to keep the UI on the screen
            screenPoint.y = Mathf.Clamp(screenPoint.y, ySize, yBound);
            transform.position = cam.ScreenToWorldPoint(screenPoint);//Moved the UI to where the mouse is
        }
        if(Input.GetAxis("RadialMenu") > 0 && UIOn == false && ButtonUp == true) {//Pressing Q makes the UI appear, if it is off
            ButtonUp = false;
            if (King.GetComponent<KingPlace>().FirstPlacing == true) {
                King.GetComponent<KingPlace>().CancelPlacing();
            }
            MenuOn();
        }
        else if (Input.GetAxis("RadialMenu") > 0 && UIOn == true && ButtonUp == true) {//Pressing Q makes the UI dissapear, if it is on
            ButtonUp = false;
            MenuOff();
        }

        if(Input.GetAxis("RadialMenu") == 0)
        {
            ButtonUp = true;
        }
    }

    private void MenuOn()
    {
        UItext.text = "Press Q to Close Menu";
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
