using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using UnityEngine;

public class Arrow : NetworkBehaviour {
    private Vector3 target;
    private bool isLive = false;
    private Vector3 dir = Vector3.zero;

    public float speed = 90f;

    [SerializeField] private float bumpPower = 30;

    //finds targed
    public void Seek(UnityEngine.Vector3 _target) {
        //can also do effects, speed on the bullet, damage amount, etc.
        target = _target;
        isLive = true;

        //find the direction to fire
        dir = (target - transform.position).normalized;

        // Start a co-routine to despawn the arrow if it doesn't hit anything
        StartCoroutine(DespawnCountdown());
    }

    IEnumerator DespawnCountdown() {
        int time = 5;

        for (int i = time; i > 0; i--) {
            yield return new WaitForSecondsRealtime(1f);
        }

        // Time's up - Despawn us | Only despawn once
        if (IsHost) {
            HitTargetServerRPC();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only move the arrow on the host
        if (!IsHost) { return; }

        // This keeps the static arrows from moving without a target
        if (target == null) { return; }

        // The arrow is allowed to move towards a target
        if (isLive) {
            
            
            // Calculate distance
            float distanceThisFrame = speed * Time.deltaTime;

            //move in the worldspace
            transform.Translate(dir * distanceThisFrame, Space.World);

        }
    }

    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider objectHit) {
        if (objectHit != null) { return; }

        if (objectHit.transform.parent.gameObject.tag == "Player") {
            //Checks if the other object is the player
            MoveStateManager playerMovement = objectHit.GetComponent<MoveStateManager>();

            Vector3 dirBump = objectHit.transform.position - transform.position;

            dirBump.y = .1f;
            if(dirBump.x == 0 && dirBump.z == 0){
                dirBump = new Vector3(1,.1f,1);
            }

            playerMovement.GetHit(dirBump.normalized, bumpPower);
            //
        }

        // If it hits anything else, we still want to despawn it

        // Despawn the arrow
        HitTargetServerRPC();
    }

    // Arrow has connected with something
    [ServerRpc]
    private void HitTargetServerRPC() {
        isLive = false;

        // Despawn the arrow
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
