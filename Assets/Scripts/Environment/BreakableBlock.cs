using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    //kicking power 500 (1000 for stornger kick)
    public float health;
    // Start is called before the first frame update
    void Start(){
        //if health is not set, default it to 1
        if(health == 0){

            health = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void damage(float kickPower){
        int totalDamage = (int)(kickPower / 100.0);
        health -= totalDamage;
        Debug.Log(health);
        //if item is at 0 health
        if(health <= 0){
            destroy();
        }
    }
    //called once health here is zero
    public void destroy(){
        Destroy(this.gameObject);
    }
}
