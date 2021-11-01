using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//may want to change this to be more connected with abillity cooldown (should be same time but)
public class UICoolDown : MonoBehaviour{
    private float currentTime;
    private float coolDown;
    private Image uiImage;
    public void setCoolDownTime(float time){
        coolDown = time;
    }
    public void startCoolDown(){
        currentTime = 0.0f;
    }
    // Start is called before the first frame update
    void Start(){
        //set current time to coo
        currentTime = coolDown;
        //get image
        uiImage = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update(){
        //if current on cooldown
        if(currentTime < coolDown){
            uiImage.fillAmount = (currentTime) / (coolDown * 1.0f);

            //update currentTime
            currentTime = currentTime + Time.deltaTime;
        }
    }
}
