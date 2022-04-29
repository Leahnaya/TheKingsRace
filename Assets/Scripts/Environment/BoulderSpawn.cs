using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawn : NetworkBehaviour
{
    //Variables to be customized as needed in engine
    [SerializeField] private float RespawnTimer = 10.0f;
    [SerializeField] private float InitialSpawnDelay = 0.0f;
    [SerializeField] private float boulderScaleMultiplier = 1.0f;
    [SerializeField] private Vector3 spawnForce;
    [SerializeField] private Vector3 SpawnLocation;
    [SerializeField] private Transform BoulderPrefab;

    //Variables for managing cleanup and spawning
    private float currentTime = 0.0f;
    private GameObject boulderInScene;

    void Start()
    {
        if (IsHost) //On startup call spawn RPC as host
        {
            SpawnBoulderServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBoulderServerRPC()
    {
        //Intantiate a boulder, rescale it, then spawn it on all clients, then start its despawn countdown
        boulderInScene = Instantiate(BoulderPrefab, SpawnLocation, Quaternion.identity).gameObject;
        boulderInScene.transform.localScale *= boulderScaleMultiplier;
        boulderInScene.GetComponent<NetworkObject>().Spawn(null, true);
        boulderInScene.GetComponent<Boulder>().StartCountdown((int)RespawnTimer, spawnForce);
    }

    private void FixedUpdate()
    {
        //Clients disregard this update
        if (!IsHost) { return; }

        //Host updates the time for respawning boulder then recalls the spawn RPC
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= RespawnTimer + InitialSpawnDelay)
        {
            currentTime = 0.0f;
            InitialSpawnDelay = 0.0f;
            SpawnBoulderServerRPC();
        }
    }

}
