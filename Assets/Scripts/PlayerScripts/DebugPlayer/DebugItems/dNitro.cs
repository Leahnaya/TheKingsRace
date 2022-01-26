using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitro : MonoBehaviour
{
    private PlayerStats playerStats;
    private CoolDown driver;
    private bool isOnCoolDown = false;
    //this will need to be set from scritable object or something;
    private float coolDown;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        //once cooldowns are implemented, put this on one (a long one)
        if (Input.GetKeyDown(KeyCode.LeftShift) && isOnCoolDown == false && playerStats.HasNitro)
        {
            playerStats.CurVel = playerStats.MaxVel;
            StartCoroutine(startCoolDown());
        }
    }

    private IEnumerator startCoolDown(){
        Debug.Log("start corotine");
        isOnCoolDown = true;
    //    driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(coolDown);
        isOnCoolDown = false;
    }
}
