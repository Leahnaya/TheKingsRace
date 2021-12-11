using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawn : MonoBehaviour
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
        boulderInScene = Instantiate(BoulderPrefab, SpawnLocation, Quaternion.identity).gameObject;
        boulderInScene.GetComponent<Rigidbody>().AddForce(spawnForce);
    }

    private void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime >= RespawnTimer)
        {
            currentTime = 0.0f;
            Destroy(boulderInScene);
            boulderInScene = Instantiate(BoulderPrefab, SpawnLocation, Quaternion.identity).gameObject;
            boulderInScene.GetComponent<Rigidbody>().AddForce(spawnForce);
        }
    }

}
