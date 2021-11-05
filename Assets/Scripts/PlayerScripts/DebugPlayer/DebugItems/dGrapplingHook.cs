using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class dGrapplingHook : NetworkBehaviour
{
    public float maxGrappleDistance = 25;

    private bool isGrappled;
    private int hookPointIndex;
    private GameObject hookPoint;
    private GameObject[] hookPoints;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        isGrappled = false; 
        hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsLocalPlayer) { return; }
        
        if (Input.GetKeyDown(KeyCode.E)) //If grapple button is hit
        {
            if (!isGrappled) //If we are not grappling
            {
                hookPointIndex = FindHookPoint(); //Find the nearest hook point within max distance
                if (hookPointIndex != -1) //If there is a hookpoint
                {
                    hookPoint = hookPoints[hookPointIndex]; //The point we are grappling from
                    //physics set up?
                    isGrappled = true; //toggle grappling state
                }
            } 
            else //Else we are grappling
            {
                //physics tear down?
                isGrappled = false; //toggle grappling state to release
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrappled)
        {
            //Do grappling physics based on hookPoint
        }
    }

    int FindHookPoint()
    {
        float least = maxGrappleDistance;
        int index = -1;
        for(int i = 0; i<hookPoints.Length; i++)
        {
            distance = Vector3.Distance(gameObject.transform.position, hookPoints[i].transform.position);
            if (distance <= least)
            {
                index = i;
            }
        }
        return index;
    }
}
