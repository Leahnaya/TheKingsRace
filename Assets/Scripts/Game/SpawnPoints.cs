using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour {

    public static SpawnPoints Instance => instance;
    private static SpawnPoints instance;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /*
     * Level IDs for returning spawn points
     * Mountain - 0
     * 
     */

    [Header("Mountain")]
    [SerializeField] private Vector3[] runnerSpawnPoints;
    [SerializeField] private Vector3 kingSpawnPoint;

    public Vector3[] getRunnerSpawnPoints(int levelID) { 
        switch(levelID) {
            default:
            case 0:
                return runnerSpawnPoints;
        }
    }

    public Vector3 getKingSpawnPoint(int levelID) {
        switch (levelID) {
            default:
            case 0:
                return kingSpawnPoint;
        }
    }
}
