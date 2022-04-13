using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KingCursor : MonoBehaviour
{
    // Start is called before the first frame update
    public int mouseSensitivity;

    //due to the nature of trigger and input handler together, it will somteimes miss time and this is necessary
    public bool inTrigger;
    public Collider2D curButton;
    public Image cursorImage;
    public CircleCollider2D circleCollider;
    void Start()
    {
        cursorImage = gameObject.GetComponent<Image>();
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //update mouse position vec 
        Vector2 mouseVec = new Vector2(0, 0);
        //if horizontal input from right analog
        if (Input.GetAxis("KingHorizontalMouseMove") != 0)
        {
            mouseVec.x = Input.GetAxis("KingHorizontalMouseMove");
        }
        //if vertical input from right analog
        if (Input.GetAxis("KingVerticalMouseMove") != 0)
        {
            //uninvert
            mouseVec.y = Input.GetAxis("KingVerticalMouseMove");
        }
        //if any input 
        if (mouseVec.magnitude != 0)
        {

            //if icon is not on turn on
            if(cursorImage.enabled == false)
            {
                cursorImage.enabled = true;
                circleCollider.enabled = true;
            }
            mouseVec = mouseVec.normalized * mouseSensitivity * Time.deltaTime;
            //make it a little more smooth
            mouseVec.x = mouseVec.x * (4f / 3f);

            this.transform.localPosition += new Vector3(mouseVec.x, mouseVec.y, 0);
            //cursor.GetComponent<RectTransform>().localPosition 
        }
        //check 
        cursorInCollider();

        //check if mouse movement if so (turn off pointer)
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            //if controller pointer on, turn it off
            if(cursorImage.enabled == true)
            {
                cursorImage.enabled = false;
                circleCollider.enabled = false;

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventSystem.current.SetSelectedGameObject(collision.gameObject);
        curButton = collision;
        inTrigger = true;

        //if hover has a pointer enter/exit thing
        if (collision.gameObject.CompareTag("hasEvent"))
        {
            try
            {
                ExecuteEvents.Execute(collision.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
            }
            catch(System.Exception e)
            {
                Debug.Log("broke event");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        EventSystem.current.SetSelectedGameObject(null);
        curButton = null;
        inTrigger = false;

        if (collision.gameObject.CompareTag("hasEvent"))
        {
            try
            {
                ExecuteEvents.Execute(collision.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            }
            catch(System.Exception e)
            {
                Debug.Log("broke event");
            }
        }
    }
    private void cursorInCollider(){
        if(inTrigger == true){
            if (Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                //if curButton is mlapi relay
                if (curButton.transform.name == "Toggle"){
                    ExecuteEvents.Execute(curButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
                //if regular button
                else {
                    curButton.gameObject.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
}
