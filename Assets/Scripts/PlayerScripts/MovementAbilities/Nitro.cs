using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour
{
    private PlayerStats playerStats;
    // Start is called before the first frame update
    void Start(){
        playerStats = this.gameObject.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update(){
        //once cooldowns are implemented, put this on one (a long one)
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            playerStats.CurVel = playerStats.MaxVel;
        }
    }
}
