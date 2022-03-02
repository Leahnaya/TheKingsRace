using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Exceptions;
using System.Linq;

public class ResetZones : MonoBehaviour {

    private Transform ValleyRespawnPoint;
    private Transform PlainsRespawnPoint;
    private Transform FoothillsRespawnPoint;
    private Transform MountainRespawnPoint;

    public enum Zone { 
        VALLEY,
        PLAINS,
        FOOTHILLS,
        MOUNTAIN
    }

    private Zone currentZone;


    void Start() {
        // Make sure we are in the game scene
        if (SceneManager.GetActiveScene().buildIndex == 3) {
            ValleyRespawnPoint = GameObject.FindGameObjectWithTag("ValleyRespawnPoint").transform;
            PlainsRespawnPoint = GameObject.FindGameObjectWithTag("PlainsRespawnPoint").transform;
            FoothillsRespawnPoint = GameObject.FindGameObjectWithTag("FoothillsRespawnPoint").transform;
            MountainRespawnPoint = GameObject.FindGameObjectWithTag("MountainRespawnPoint").transform;
        }

        // Starting zone is the valley
        currentZone = Zone.VALLEY;
    }

    public void SetCurrentZone(int newZone) {
        currentZone = (Zone)newZone;
    }

    public Zone GetCurrentZone() {
        return currentZone;
    }

    public Vector3 GetRespawnPosition(Zone zone) {
        switch(zone) {
            case Zone.PLAINS:
                return PlainsRespawnPoint.position;
            case Zone.FOOTHILLS:
                return FoothillsRespawnPoint.position;
            case Zone.MOUNTAIN:
                return MountainRespawnPoint.position;
            default:
            case Zone.VALLEY:
                return ValleyRespawnPoint.position;
        }
    }
}
