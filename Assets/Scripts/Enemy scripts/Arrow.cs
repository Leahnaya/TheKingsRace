using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Arrow : NetworkBehaviour {
    private Vector3 target;
    private bool isLive = false;

    public float speed = 90f;

    //finds targed
    public void Seek(UnityEngine.Vector3 _target) {
        //can also do effects, speed on the bullet, damage amount, etc.
        target = _target;
        isLive = true;
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
            // Find the direction to fire in
            Vector3 dir = target - transform.position;
            
            // Calculate distance
            float distanceThisFrame = speed * Time.deltaTime;


            // Checking from current distance to current target
            if (dir.magnitude <= distanceThisFrame) {
                // NOT SURE IF THIS CALC ^ WORKS AS INTENDED
                HitTargetServerRPC();
                return;
            }

            //move in the worldspace
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        }
    }

    // Arrow has connected with something
    [ServerRpc]
    private void HitTargetServerRPC() {
        isLive = false;

        // Do the hit logic

        // Despawn the arrow
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

}
