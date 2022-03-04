using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;

public class WeatherWheel : NetworkBehaviour {

    private float wheelSpeed;
    private float subtractSpeed;
    public bool isSpinning = false;

    public GameObject pointer;

    void Start() {
        gameObject.GetComponent<Image>().enabled = false;
        pointer.SetActive(false);
        StartCoroutine(ShowWeatherWheel());
    }

    IEnumerator ShowWeatherWheel() {
        // Get the time to complete the intro camera fly through
        float cameraLerpTime = Camera.main.GetComponent<CPC_CameraPath>().playOnAwakeTime;

        // Wait for that many seconds - allows for time to complete the "cutscene"
        yield return new WaitForSecondsRealtime(cameraLerpTime);
        
        // Turn the wheel back on for the 

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            ulong clientID = player.GetComponent<NetworkObject>().OwnerClientId;
            if (clientID == NetworkManager.Singleton.LocalClientId) {
                if (player.GetComponentInChildren<PlayerStats>() != null) {
                    // Runner
                    gameObject.SetActive(false);
                } else {
                    // King
                    gameObject.GetComponent<Image>().enabled = true;
                    gameObject.GetComponent<Button>().interactable = true;
                    pointer.SetActive(true);
                }
            }
        }
    }

    void Update()
    {

        if (isSpinning)
        {
            transform.Rotate(0, 0, wheelSpeed);
            wheelSpeed -= subtractSpeed;
        }

        // Check when stopped spinning
        if (isSpinning && wheelSpeed <= 0)
        {
            wheelSpeed = 0;
            isSpinning = false;

            // Check the angle to see what won
            float finalAngle = transform.rotation.eulerAngles.z;

            if (finalAngle <= 45 || finalAngle > 315)
            {
                // Snow
                SpawnWeatherServerRPC(PlayerStats.Weather.Snow);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (finalAngle > 45 && finalAngle <= 135)
            {
                // Windy

                // Calculate the wind direction
                Vector3 windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

                SpawnWeatherServerRPC(PlayerStats.Weather.Wind, windDirection);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (finalAngle > 135 && finalAngle <= 225)
            {
                // Fog
                SpawnWeatherServerRPC(PlayerStats.Weather.Fog);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else if (finalAngle > 225 && finalAngle <= 315)
            {
                // Rain
                SpawnWeatherServerRPC(PlayerStats.Weather.Rain);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }

            // Start an IEnum to turn off the weather
            //TODO: here
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnWeatherServerRPC(PlayerStats.Weather weather, Vector3 windDir = default(Vector3)) {
        SpawnWeatherClientRPC(weather, windDir);
    }

    [ClientRpc]
    private void SpawnWeatherClientRPC(PlayerStats.Weather weather, Vector3 windDir = default(Vector3)) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Apply the weather affects to the player
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                // This check makes it so the king isn't affected
                if (player.GetComponentInChildren<PlayerStats>() != null) {
                    player.GetComponentInChildren<PlayerStats>().SetWeather(weather, windDir);
                }
            }
        }

        // Turn on the weather particle systems
        switch (weather) {
            case PlayerStats.Weather.Rain:
                GameObject.FindGameObjectWithTag("RainSystem").GetComponent<ParticleSystem>().Play();
                break;
            case PlayerStats.Weather.Snow:
                GameObject.FindGameObjectWithTag("SnowSystem").GetComponent<ParticleSystem>().Play();
                break;
            case PlayerStats.Weather.Wind:
                GameObject.FindGameObjectWithTag("WindSystem").GetComponent<ParticleSystem>().Play();
                break;
            case PlayerStats.Weather.Fog:
                GameObject.FindGameObjectWithTag("FogSystem").GetComponent<ParticleSystem>().Play();
                break;
        }
    }

    public void SpinWheel() {

        // Only spin if not spinning already
        if (isSpinning) { return; }

        // Do the spin
        wheelSpeed = Random.Range(4.000f, 5.000f);
        subtractSpeed = Random.Range(0.003f, 0.009f);
        isSpinning = true;
    }
}
