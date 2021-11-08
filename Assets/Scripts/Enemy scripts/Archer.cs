using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    //current target ponting at
    public Transform target;


    //range of turrent
    public float range = 20.0f;

    //how fast and how much time before the next shot
    public float fireRate = 4.0f;
    private float shootingCooldown = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, range);

    }
}
