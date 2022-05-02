using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingZoom : MonoBehaviour
{

    Camera cam;
    Quaternion BaseRot;

    public GameObject Panel;
    public GameObject Text;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();//Gets the camera and its rotation
        BaseRot = transform.rotation;
    }

    bool Zoomed = false;
    float RotSpeed = .5f;
    float minXAngle = 40f;
    float maxXAngle = 70f;
    float minYAngle = -45f;
    float maxYAngle = 45f;

    float XAng, YAng;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))//Switches in and out of zoom
        {
            if (Zoomed == false)
            {
                BaseRot = transform.rotation;
                cam.fieldOfView = 15;//Zooms in
                Panel.transform.localPosition = new Vector3(Panel.transform.localPosition.x, Panel.transform.localPosition.y, 4150f);//Moves the UI Text in with the zoom
                Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, Text.transform.localPosition.y, 4150f);
                Zoomed = true;
            }
            else if (Zoomed == true)
            {
                cam.fieldOfView = 40;//Zooms out
                Panel.transform.localPosition = new Vector3(Panel.transform.localPosition.x, Panel.transform.localPosition.y, 1510f);//Moves the UI Text out with the zoom
                Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, Text.transform.localPosition.y, 1510f);
                transform.rotation = BaseRot;
                Zoomed = false;
            }
        }
        if(Zoomed == true)//If Zoomed in allows the King to rotate their veiw up and down
        {
            Vector3 MosPos = Input.mousePosition;
            if (MosPos.y < 100)//Looks down
            {
                transform.Rotate(RotSpeed, 0, 0);
                XAng = Mathf.Clamp(cam.transform.localEulerAngles.x, minXAngle, maxXAngle);
                cam.transform.localEulerAngles = new Vector3(XAng, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
            }
            if (MosPos.y > 1000)//Looks up
            {
                transform.Rotate(-RotSpeed, 0, 0);
                XAng = Mathf.Clamp(cam.transform.localEulerAngles.x, minXAngle, maxXAngle);
                cam.transform.localEulerAngles = new Vector3(XAng, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
            }
        }

    }
}
