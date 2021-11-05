using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderstorm : MonoBehaviour
{
    //DESIGN DECISION, does this ability happen once or last for a DURATION (SAY 30 SECONDS?)


        // Should function as follows
            //Attaches to player (target)
            //While target is above 'Y' height
                //spawn particle system
            //If player touches ground before 5 seconds have passed
                //No zap
            //Else if player doesn't touch ground
                //Zap em, hit them with a strong downwards impact, and ragdoll them for 3 seconds (2 if fast standup?)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
