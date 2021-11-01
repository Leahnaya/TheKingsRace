using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class SpecialItem : Item
{
    public bool hasWallrunM;
    public bool hasBlinkM;
    public bool hasGrappleM;
    public bool hasGliderM;
    public bool hasNitroM;
    public bool hasDashM;
    public float cooldownM;
    
    public override void Equip(PlayerStats p, GameObject player){
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
        if(hasWallrunM != false){
            p.HasWallrun = hasWallrunM;
        }
        if(hasBlinkM != false){
            p.HasBlink = hasBlinkM;
        }
        if(hasGrappleM != false){
            p.HasGrapple = hasGrappleM;
        }
        if(hasGliderM != false){
            p.HasGlider = hasGliderM;
        }
        if(hasNitroM != false){
            p.HasNitro = hasNitroM;
        }
        if(hasDashM != false){
            p.HasDash = hasDashM;
        }
    }

    public override void Unequip(PlayerStats p, GameObject player){
        if(maxVelM != 0){
            p.MaxVel -= maxVelM;
        }
        if(minVelM != 0){
            p.MinVel -= minVelM;
        }
        if(curVelM != 0){
            p.CurVel -= curVelM;
        }
        if(accM != 0){
             p.Acc -= accM;
        }
        if(jumpPowM != 0){
             p.JumpPow -= jumpPowM;
        }
        if(jumpNumM != 0){
            p.JumpNum -= jumpNumM;
        }
        if(tractionM != 0){
            p.Traction -= tractionM;
        }
        if(kickPowM != 0){
            p.KickPow -= kickPowM;
        }
        if(playerGravM != 0){
            p.PlayerGrav -= playerGravM;
        }
        if(hasWallrunM != false){
            p.HasWallrun = false;
        }
        if(hasBlinkM != false){
            p.HasBlink = false;
        }
        if(hasGrappleM != false){
            p.HasGrapple = false;
        }
        if(hasGliderM != false){
            p.HasGlider = false;
        }
        if(hasNitroM != false){
            p.HasNitro = false;
        }
        if(hasDashM != false){
            p.HasDash = false;
        }

    }
}

