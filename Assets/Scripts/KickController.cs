using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickController : MonoBehaviour
{
    //Important, may want to change blink code to only work for trigger collider 
    // so that the player doesn't teleport to their leg
    public GameObject leg;
    private bool isKicking = false;

    void Update(){
        //if k is pressed enable leg for .3 seconds 
        if(Input.GetKeyDown(KeyCode.F) && isKicking==false){
            StartCoroutine(Kicking(.3f));
        }
    }


    private IEnumerator Kicking(float waitTime)
    {
        isKicking = true;
        leg.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        isKicking = false;
        leg.SetActive(false);

    }

}
