using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitro : MonoBehaviour
{
    private CoolDown driver;
    private PlayerStats playerStats;
    public SpecialItem nitroItem;
    private bool isOnCoolDown = false;
    public bool isNitroing = false;

    private float tempTimer = 5;
    //this will need to be set from scritable object or something;

    // Start is called before the first frame update
    void Start()
    {
        //driver = GameObject.Find("Canvas").GetComponent<CoolDown>();
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        //once cooldowns are implemented, put this on one (a long one)
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton8)) && isOnCoolDown == false && playerStats.HasNitro)
        {
            isNitroing = true;
            
        }
    }

    void FixedUpdate(){
        if(isNitroing){
            if(tempTimer > 0){
                
                tempTimer -= .02f;

                Debug.Log("nitro is on");

                if(playerStats.CurVel < playerStats.HardCapMaxVel){
                    playerStats.CurVel += playerStats.Acc * 50;
                }
                else if(playerStats.CurVel > playerStats.HardCapMaxVel){
                    playerStats.CurVel = playerStats.HardCapMaxVel;
                }
            }
            else{
                StartCoroutine(startCoolDown());
                isNitroing = false;
            }
        }
    }

    private IEnumerator startCoolDown(){
        Debug.Log("start corotine");
        isOnCoolDown = true;
        //driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(nitroItem.cooldownM);
        isOnCoolDown = false;
        tempTimer = 5;
        Debug.Log("end corotine");
    }
}
