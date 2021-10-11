using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunItem : Item
{
    public void Equip(PlayerStats p){
        p.HasWallrun = true;
    }

    public void Unequip(PlayerStats p)
    {
        p.HasWallrun = false;
    }
}
