using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePlatform : AbstractInteractable
{
    //this function currently doesn't raycast on button click (stubbed with button clicks for now)
    public override void OnInteract()
    {
        //if button 1 down
        if (Input.GetKeyDown((KeyCode.Alpha1)))
        {
            Debug.Log("Item gone");
            Destroy(interactableObjects[0]);
        }
        if (Input.GetKeyDown((KeyCode.Alpha2)))
        {
            Debug.Log("Item gone");
            Destroy(interactableObjects[1]);
        }
        if (Input.GetKeyDown((KeyCode.Alpha3)))
        {
            Debug.Log("Item gone");
            Destroy(interactableObjects[2]);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //temporary
        OnInteract();
    }
}
