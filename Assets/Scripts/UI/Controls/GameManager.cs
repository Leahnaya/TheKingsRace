using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    //values for runner
    //probably shouldn't rebind this
    //public KeyCode glide;
    [SerializeField]
    public Dictionary<string,KeyCode> bindableActions = new Dictionary<string, KeyCode>();

    private void Awake()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(gameObject);
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }
        //

        //for keyboard
        /*        walkForward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("walkForwardKey", "W"));
                walkBackward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("walkBackwardKey", "S"));
                walkLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("walkLeftKey", "A"));
                walkRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("walkRightKey", "D"));*/
        //KeyCode jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKey", "Space"));
        KeyCode slide = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("slideKey", "Q"));
        KeyCode kick = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("kickKey", "F"));
        KeyCode dash = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("dashKey", "R"));
        KeyCode nitro = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("nitroKey", "LeftShift"));
        KeyCode grapple = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("grappleKey", "E"));


        //bindableActions.Add("jumpKey", jump);
        bindableActions.Add("slideKey", slide);
        bindableActions.Add("kickKey", kick);
        bindableActions.Add("dashKey", dash);
        bindableActions.Add("nitroKey", nitro);
        bindableActions.Add("grappleKey", grapple);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
