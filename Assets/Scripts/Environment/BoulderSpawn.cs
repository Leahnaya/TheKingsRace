using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawn : NetworkBehaviour
{
    [SerializeField] private float RespawnTimer = 10.0f;
    private float currentTime = 0.0f;

    [SerializeField] private Vector3 spawnForce;

    [SerializeField] private Vector3 SpawnLocation;

    [SerializeField] private Transform BoulderPrefab;
    private GameObject boulderInScene;

    // Start is called before the first frame update
    void Start()
    {
        if (IsHost)
        {
            SpawnBoulderServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBoulderServerRPC() {
        boulderInScene = Instantiate(BoulderPrefab, SpawnLocation, Quaternion.identity).gameObject;
        boulderInScene.GetComponent<NetworkObject>().Spawn(null, true);
        boulderInScene.GetComponent<Boulder>().StartCountdown((int)RespawnTimer, spawnForce);
    }

    private void FixedUpdate()
    {
        if (!IsHost) { return; }

        currentTime += Time.fixedDeltaTime;
        if (currentTime >= RespawnTimer)
        {
            currentTime = 0.0f;
            SpawnBoulderServerRPC();
        }
    }

}
