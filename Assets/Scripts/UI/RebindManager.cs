using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebindManager : MonoBehaviour
{
    Event keyEvent;
    KeyCode newKey;
    GameObject currentButtonObject;
    //const array of valid input
    readonly KeyCode[] valildKeys = {KeyCode.Q, KeyCode.E, KeyCode.R, KeyCode.F, KeyCode.LeftShift, KeyCode.Space};


    //currently waiting for input from user
    bool waitingForKey;
    bool hasPressedValidKey = false;
    // Start is called before the first frame update
    void Start()
    {
        waitingForKey = false;
        //go through all buttons and set correct image
        //for
 



    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnGUI()
    {
        keyEvent = Event.current;
        //if is a key event
        if (keyEvent.isKey)
        {
            //if waiting for a key and the key entered is valid
            if (waitingForKey && isValidKey(keyEvent.keyCode))
            {
                newKey = keyEvent.keyCode;
                waitingForKey = false;
                hasPressedValidKey = true;
                //Debug.Log("right key");
            }
        }
    }
    public void SendCurrentButton(GameObject gameObject)
    {
        currentButtonObject = gameObject;
    }
    public void StartAssignment(string keyName)
    {
        if (!waitingForKey)
        {
            StartCoroutine(AssignKey(keyName));
        }
    }

    //
    IEnumerator waitForKey()
    {
        //if it gets past here it has a keycode, but we also want to double check if it is one of the 10 keys we 
        //TO-DO add controller keys
        //Debug.Log("waiting");
        while (hasPressedValidKey == false)
        {
            yield return null;
        }
    }
    public bool isValidKey(KeyCode keycode)
    {
        bool hasValue = false;
        for(int i =0; i < valildKeys.Length; i++) {
            if(valildKeys[i] == keycode)
            {
                hasValue = true;
            }
        }
        return hasValue;
    }
    public bool areThereDuplicates()
    {
        bool duplicateExists = false;
        List<KeyCode> tHolder = new List<KeyCode>();
        foreach (var item in GameManager.GM.bindableActions)
        {
            tHolder.Add(item.Value);
        }

      /*  foreach (var item in tHolder)
        {
            Debug.Log(item);
        }*/

        for(int i=0; i < tHolder.Count - 1; i++)
        {
            for(int j=i+1; j < tHolder.Count; j++)
            {
                if(tHolder[i] == tHolder[j])
                {

                    duplicateExists = true;
                }
            }
        }
        return duplicateExists;
    }
    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;
        yield return waitForKey();

        //could be a bit more dyanmic, but it is sufficeint solution for goal
        switch (keyName)
        {
            //player prefs are used to keep key binds after game has closed (won't do anything until then). They are commented out for now for SGX
            case "kick":
                GameManager.GM.bindableActions["kickKey"] = newKey;
                //replace image of button
                //currentButtonObject.GetComponent<Image>().sprite =
                //PlayerPrefs.SetString("kickKey", GameManager.GM.bindableActions["kickKey"].ToString());
                //revert flag
                hasPressedValidKey = false;
                break;
            case "slide":
                GameManager.GM.bindableActions["slideKey"] = newKey;
                //replace image of button
                //currentButtonObject.GetComponent<Image>().sprite =
                //PlayerPrefs.SetString("slideKey", GameManager.GM.bindableActions["slideKey"].ToString());
                //revert flag
                hasPressedValidKey = false;
                break;
            case "dash":
                 GameManager.GM.bindableActions["dashKey"] = newKey;
                //replace image of button
                //currentButtonObject.GetComponent<Image>().sprite =
                //PlayerPrefs.SetString("dashKey", GameManager.GM.bindableActions["dashKey"].ToString());
                //revert flag
                hasPressedValidKey = false;
                break;
            case "nitro":
                GameManager.GM.bindableActions["nitroKey"] = newKey;
                //replace image of button
                //currentButtonObject.GetComponent<Image>().sprite =
                //PlayerPrefs.SetString("nitroKey", GameManager.GM.bindableActions["nitroKey"].ToString());
                //revert flag
                hasPressedValidKey = false;
                break;
            case "grapple":
                GameManager.GM.bindableActions["grappleKey"] = newKey;
                //replace image of button
                //currentButtonObject.GetComponent<Image>().sprite =
                //PlayerPrefs.SetString("grappleKey", GameManager.GM.bindableActions["grappleKey"].ToString());
                //revert flag
                hasPressedValidKey = false;
                break;
        }
        yield return null;
    }
}
