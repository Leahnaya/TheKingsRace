using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    private float tractionDefault;
    //Options for reduction
        // % based aka you have 75% traction while in rain <- with this option we wouldn't need to care about individual players starting points
        // Flat reduction penalty of number i.e. 3?


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag =="Player") //In multiplayer we may need a player 1 - X tag for keeping track of individual stats
        {
            isColliding = true;
            PlayerStats pStats = other.gameObject.GetComponent<PlayerStats>();
            tractionDefault = pStats.Traction;
            if (tractionDefault > 3)
            {
                pStats.Traction -= 3;
            } else
            {
                pStats.Traction = 1;
            }
            //pStats.Traction *= 0.75f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStats pStats = other.gameObject.GetComponent<PlayerStats>();
            pStats.Traction = tractionDefault;
            //pStats.Traction /= 0.75f;
        }
    }
}
