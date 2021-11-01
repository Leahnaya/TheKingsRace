using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    private float tractionDefault;
    private float accelerationDefault;
    //Options for increase
    // % based aka you have 75% traction while in rain <- with this option we wouldn't need to care about individual players starting points
    // Flat reduction penalty of number i.e. 3?

    //snow reduces acceleration <- playtest to consider adjusting max speed as well

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") //In multiplayer we may need a player 1 - X tag for keeping track of individual stats
        {
            PlayerStats pStats = other.gameObject.GetComponent<PlayerStats>();
            tractionDefault = pStats.Traction;
            if (tractionDefault < 8)
            {
                pStats.Traction += 3;
            }
            else
            {
                pStats.Traction = 10;
            }

            accelerationDefault = pStats.Acc;
            if (accelerationDefault > 3)
            {
                pStats.Acc -= 3;
            }
            else
            {
                pStats.Acc = 1;
            }

            //pStats.Traction *= 1.25f;
            //pStats.Acc *= 0.75f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerStats pStats = other.gameObject.GetComponent<PlayerStats>();
            pStats.Traction = tractionDefault;
            pStats.Acc = accelerationDefault;
            //pStats.Traction /= 1.25f;
            //pStats.Acc /= 0.75f;
        }
    }
}
