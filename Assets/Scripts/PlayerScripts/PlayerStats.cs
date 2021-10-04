using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Maximum possible player speed
    private float maxVel;
    public float MaxVel{
        get{ return maxVel; }
        set{ maxVel = value; }
    }

    //Minimum possible player speed
    private float minVel;
    public float MinVel{
        get{ return minVel; }
        set{ minVel = value; }
    }
    
    //Current player speed
    private float curVel;
    public float CurVel{
        get{ return curVel; }
        set{ curVel = value; }
    }

    //Player Jump power
    private float acc;
    public float Acc{
        get{ return acc; }
        set{ acc = value; }
    }

    //Player Jump power
    private float jumpPow;
    public float JumpPow{
        get{ return jumpPow; }
        set{ jumpPow = value; }
    }

    //Players Number of Jumps
    private int jumpNum;
    public int JumpNum{
        get{ return jumpNum; }
        set{ jumpNum = value; }
    }

    //Player Traction
    private float traction;
    public float Traction{
        get{ return traction; }
        set{ traction = value; }
    }

    //Player Kick Power
    private float kickPow;
    public float KickPow{
        get{ return kickPow; }
        set{ kickPow = value; }
    }

    //Player Recovery Time When Knocked Down 
    private float recovTime;
    public float RecovTime{
        get{ return recovTime; }
        set{ recovTime = value; }
    }

    //MAY BE UNNECCESARY DEPENDING ON HOW GLIDER TURNS OUT
    //Gravity Affecting the player
    private float playerGrav;
    public float PlayerGrav{
        get{ return playerGrav; }
        set{ playerGrav = value; }
    }

    //If The Player Has Blink
    private bool hasBlink;
    public bool HasBlink{
        get{ return hasBlink; }
        set{ hasBlink = value; }
    }

    //If The Player Has Glider
    private bool hasGlider;
    public bool HasGlider{
        get{ return hasGlider; }
        set{ hasGlider = value; }
    }

    //If The Player Has Grapple
    private bool hasGrapple;
    public bool HasGrapple{
        get{ return hasGrapple; }
        set{ hasGrapple = value; }
    }

    //If The Player Has Wallrun
    private bool hasWallrun;
    public bool HasWallrun{
        get{ return hasWallrun; }
        set{ hasWallrun = value; }
    }
}
