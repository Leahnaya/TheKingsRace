using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private PlayerStats pstats;
    public bool hasWallRun;
    [Range( 1, 50)]
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        pstats = this.gameObject.GetComponent<PlayerStats>();
        pstats.HasWallrun = hasWallRun;
        pstats.MaxVel = maxSpeed;
    }
}
