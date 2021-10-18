using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Maximum possible player speed
    [SerializeField]
    private float maxVel = 25.0f;
    public float MaxVel{
        get{ return maxVel; }
        set{ maxVel = value; }
    }

    //Minimum possible player speed
    [SerializeField]
    private float minVel = 2.0f;
    public float MinVel{
        get{ return minVel; }
        set{ minVel = value; }
    }
    
    //Current player speed
    [SerializeField]
    private float curVel = 0.0f;
    public float CurVel{
        get{ return curVel; }
        set{ curVel = value; }
    }

    //Player Jump power
    [SerializeField]
    private float acc = 0.1f;
    public float Acc{
        get{ return acc; }
        set{ acc = value; }
    }

    //Player Jump power
    [SerializeField]
    private float jumpPow = 300.0f;
    public float JumpPow{
        get{ return jumpPow; }
        set{ jumpPow = value; }
    }

    //Players Number of Jumps
    [SerializeField]
    private int jumpNum = 2; ////// UPDATE TO 2 WHEN THE JUMP ISSUE IS FIXED
    public int JumpNum{
        get{ return jumpNum; }
        set{ jumpNum = value; }
    }

    //Player Traction
    [SerializeField]
    private float traction = 3.0f;
    public float Traction{
        get{ return traction; }
        set{ traction = value; }
    }

    //Player Kick Power
    [SerializeField]
    private float kickPow;
    public float KickPow{
        get{ return kickPow; }
        set{ kickPow = value; }
    }

    //Player Recovery Time When Knocked Down
    [SerializeField] 
    private float recovTime;
    public float RecovTime{
        get{ return recovTime; }
        set{ recovTime = value; }
    }

    //MAY BE UNNECCESARY DEPENDING ON HOW GLIDER TURNS OUT
    //Gravity Affecting the player
    [SerializeField]
    private float playerGrav = 20.0f;
    public float PlayerGrav{
        get{ return playerGrav; }
        set{ playerGrav = value; }
    }

    //If The Player Has Blink
    [SerializeField]
    private bool hasBlink = false;
    public bool HasBlink{
        get{ return hasBlink; }
        set{ hasBlink = value; }
    }

    //If The Player Has Glider
    [SerializeField]
    private bool hasGlider = false;
    public bool HasGlider{
        get{ return hasGlider; }
        set{ hasGlider = value; }
    }

    //If The Player Has Grapple
    [SerializeField]
    private bool hasGrapple = false;
    public bool HasGrapple{
        get{ return hasGrapple; }
        set{ hasGrapple = value; }
    }

    //If The Player Has Wallrun
    [SerializeField]
    private bool hasWallrun = false;
    public bool HasWallrun{
        get{ return hasWallrun; }
        set{ hasWallrun = value; }
    }

    //If The Player Has Nitro
    [SerializeField]
    private bool hasNitro = false;
    public bool HasNitro{
        get{ return hasNitro; }
        set{ hasNitro = value; }
    }

    //If The Player Has Dash
    [SerializeField]
    private bool hasDash = false;
    public bool HasDash{
        get{ return hasDash; }
        set{ hasDash = value; }
    }

    [SerializeField]
    private int playerPoints = 15;
    public int PlayerPoints{
        get{ return playerPoints; }
        set{ playerPoints = value; }
    }
}
