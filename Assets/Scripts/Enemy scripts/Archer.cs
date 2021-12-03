using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    //current target ponting at
    private Transform target;

    public float rotationSpeed = 10f;

    //range of turrent
    public float range = 20.0f;

    //how fast and how much time before the next shot
    public float fireRate = 4.0f;
    private float shootingCooldown = 2.0f;

    public string runnerTag = "Player"; //tags the player;

    public Transform partToRotate;


    // Start is called before the first frame update
    void Start()
    {
        //setting updateTarget to be called 2 times a second

        InvokeRepeating("updateTarget",0f,0.5f);
    }

    void updateTarget()
    {
        //cycles through all enemies within range, the closest one, and sets the target at.
        //not done every frame
        
        GameObject[] runners = GameObject.FindGameObjectsWithTag(runnerTag);

        //temp variable for the shortest distance for an runner
        float shortestDistance = Mathf.Infinity;

        //temp variable for the nearest runner
        GameObject nearestRunner = null;

        foreach(GameObject runner in runners)
        {
            float distanceToRunner = Vector3.Distance(transform.position, runner.transform.position);

            if(distanceToRunner < shortestDistance)
            {
                shortestDistance = distanceToRunner;
                nearestRunner = runner;
            }
        }

        if(nearestRunner != null && shortestDistance <= range)
        {
            target = nearestRunner.transform;
        }
        else
        {
            target = null; //out of range? deselects
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return; //no target, does nothing
        }
        //b - a
        Vector3 dir = (target.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(dir);//how to rotate to look that way.
        //convert into a VEC 3 from Quaternion
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime *rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler (0f, rotation.y, 0f);
        
    }

    void OnDrawGizmosSelected()
    {

    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, range); 
    //^ this shows the range of the archer, only in editor.

    }
}
