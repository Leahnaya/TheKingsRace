using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using UnityEngine;

public class Arrow : NetworkBehaviour {
    private Vector3 target;
    private bool isLive = false;

    public float speed = 90f;

    [SerializeField] private float bumpPower = 30;

    //finds targed
    public void Seek(UnityEngine.Vector3 _target) {
        //can also do effects, speed on the bullet, damage amount, etc.
        target = _target;
        isLive = true;

        // Start a co-routine to despawn the arrow if it doesn't hit anything
        StartCoroutine(DespawnCountdown());
    }

    IEnumerator DespawnCountdown() {
        int time = 15;

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
            // Find the direction to fire in
            Vector3 dir = target - transform.position;
            
            // Calculate distance
            float distanceThisFrame = speed * Time.deltaTime;

            //move in the worldspace
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        }
    }

    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider objectHit) {
        if (objectHit.transform.parent.gameObject.tag == "Player") {
            //Checks if the other object is the player
            PlayerMovement playerMovement = objectHit.GetComponent<PlayerMovement>();

            float DirBumpX = playerMovement.driftVel.x * -1;//Inverts the Player Velocity x
            float DirBumpY = .1f;
            float DirBumpZ = playerMovement.driftVel.z * -1;//Inverts the Player Velocity z

            Vector3 DirBump = new Vector3(DirBumpX, DirBumpY, DirBumpZ);//Creates a direction to launch the player
            DirBump = Vector3.Normalize(DirBump);//Normalizes the vector to be used as a bump direction

            playerMovement.GetHit(DirBump, bumpPower);
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
