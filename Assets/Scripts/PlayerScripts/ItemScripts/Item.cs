using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item: ScriptableObject {
    public int id;
    public string itemName;
    public string description;
    [Space]
    public float maxVelM;
    public float minVelM;
    public float curVelM;
    public float accM;
    public float jumpPowM;
    public int jumpNumM;
    public float tractionM;
    public float kickPowM;
    public float recovTimeM;
    public float playerGravM;

    public void Equip(PlayerStats p){
        if(maxVelM != 0){
            p.MaxVel += maxVelM;
        }
        if(minVelM != 0){
            p.MinVel += minVelM;
        }
        if(curVelM != 0){
            p.CurVel += curVelM;
        }
        if(accM != 0){
             p.Acc += accM;
        }
        if(jumpPowM != 0){
             p.JumpPow += jumpPowM;
        }
        if(jumpNumM != 0){
            p.JumpNum += jumpNumM;
        }
        if(tractionM != 0){
            p.Traction += tractionM;
        }
        if(kickPowM != 0){
            p.KickPow += kickPowM;
        }
        if(playerGravM != 0){
            p.PlayerGrav += playerGravM;
        }
    }
}
