using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour
{
    private CoolDown driver;
    private PlayerStats playerStats;
    public SpecialItem nitroItem;
    private bool isOnCoolDown = false;
    //this will need to be set from scritable object or something;

    // Start is called before the first frame update
    void Start()
    {
        driver = GameObject.Find("Canvas").GetComponent<CoolDown>();
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        //once cooldowns are implemented, put this on one (a long one)
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton8)) && isOnCoolDown == false && playerStats.HasNitro){
            playerStats.CurVel = playerStats.MaxVel;
            StartCoroutine(startCoolDown());
        }
    }

    private IEnumerator startCoolDown(){
        Debug.Log("start corotine");
        isOnCoolDown = true;
        driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(nitroItem.cooldownM);
        isOnCoolDown = false;
    }
}
