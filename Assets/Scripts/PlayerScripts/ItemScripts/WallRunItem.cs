using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunItem : Item
{
    public override void Equip(PlayerStats p){
        p.HasWallrun = true;
    }

    public override void Unequip(PlayerStats p)
    {
        p.HasWallrun = false;
    }
}
