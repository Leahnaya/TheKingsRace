using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : NetworkBehaviour
{
    //current target ponting at
    private Transform target;
    
    [Header("Attributes")]
    //range of turret
    [SerializeField]
    private float range;

    //how fast and how much time before the next shot
    public float fireRate = 2f;
    private float shootingCooldown = 1.0f;

    [Header("Unity Setup Fields")]
    public float rotationSpeed = 10f;

    public Transform ArrowPrefab;
    public Transform firePoint;

    private GameObject arrowInScene;

    // Start is called before the first frame update
    void Start() {
        // Only the host should update targetting
        if (IsHost) { 
            //setting updateTarget to be called 2 times a second
            InvokeRepeating("updateTarget",0f,0.5f);
        }
    }

    void updateTarget()  {
        //cycles through all enemies within range, the closest one, and sets the target at.
        //not done every frame
        
        GameObject[] runners = GameObject.FindGameObjectsWithTag("ArcherTarget");

        //temp variable for the shortest distance for an runner
        float shortestDistance = Mathf.Infinity;

        //temp variable for the nearest runner
        GameObject nearestRunner = null;

        foreach(GameObject runner in runners)
        {
            //Debug.Log(runners.Length);

            float distanceToRunner = Vector3.Distance(transform.position, runner.transform.position);

            if(distanceToRunner < shortestDistance)
            {
                shortestDistance = distanceToRunner;
                nearestRunner = runner;
            }
        }

        if(nearestRunner != null && shortestDistance <= range)
        {
            target = nearestRunner.transform; //adding in the players rotation and momentum.

            //TODO: leads the shots here
        }
        else
        {
            target = null; //out of range? deselects
        }

    }
    // Update is called once per frame
    void Update() {

        // Shooting Checks are done here, so return and do nothing if not host OR no target

        if (!IsHost || target == null) { return; }

        //b - a
        Vector3 dir = (target.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(dir);//how to rotate to look that way.
        //convert into a VEC 3 from Quaternion
        Vector3 rotation = Quaternion.Lerp(this.gameObject.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        //Debug.Log("Look Rot: " + lookRotation + " Rot: " + rotation);
        this.gameObject.transform.rotation = Quaternion.Euler (0f, rotation.y, 0f);
        
        // Actually shoot it
        
        if (target != null && shootingCooldown <= 0f) {
            Vector3 position = target.position;
            ShootArrowServerRPC(position);
            shootingCooldown = 1f/ fireRate;
        }

        shootingCooldown -= Time.deltaTime;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootArrowServerRPC(Vector3 tar) {

        if (tar == null) { return; }

        arrowInScene = Instantiate(ArrowPrefab, firePoint.position, firePoint.rotation).gameObject; //Note from vinny - Maybe put this bit in the if check? seems that if target goes out of range while the RPC is being called is when we get null refs, maybe have a bool toggle that makes it so we dont look for new targets while we are still shooting through the rpc?
        arrowInScene.GetComponent<NetworkObject>().Spawn(null, true);
        
        arrowInScene.GetComponent<Arrow>().Seek(tar);
    }



    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range); 
        //^ this shows the range of the archer, only in editor.
    }
}
