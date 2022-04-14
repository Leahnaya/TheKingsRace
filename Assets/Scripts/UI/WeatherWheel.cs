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
    private bool onCooldown = false;

    public GameObject pointer;

    private float weatherDuration = 20f;
    private float postWeatherCooldown = 10f;

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
                    pointer.GetComponent<Button>().interactable = true;
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
                // Windy

                // Calculate the wind direction
                Vector3 windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

                SpawnWeatherServerRPC(PlayerStats.Weather.Wind, windDirection);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (finalAngle > 45 && finalAngle <= 135)
            {
                // Snow
                SpawnWeatherServerRPC(PlayerStats.Weather.Snow);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (finalAngle > 135 && finalAngle <= 225)
            {
                // Rain
                SpawnWeatherServerRPC(PlayerStats.Weather.Rain);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else if (finalAngle > 225 && finalAngle <= 315)
            {
                // Fog
                SpawnWeatherServerRPC(PlayerStats.Weather.Fog);

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }

            // Start an IEnum to turn off the weather
            StartCoroutine(StopWeatherCountdown());
        }
    }

    IEnumerator StopWeatherCountdown() { 
        // Weather duration
        for (int i = 0; i < weatherDuration; i++) {
            yield return new WaitForSecondsRealtime(1f);
        }

        // Stop weather after duration
        StopWeatherServerRPC();

        // Post weather cool down
        for (int i = 0; i < postWeatherCooldown; i++) {
            yield return new WaitForSecondsRealtime(1f);
        }

        // Allow weather to be spun again
        onCooldown = false;
        gameObject.GetComponent<Button>().interactable = true;
        pointer.GetComponent<Button>().interactable = true;
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

                // Also store the wind direction
                GameObject.FindGameObjectWithTag("WindSystem").GetComponent<WindDirection>().windDireciton = windDir;
                break;
            case PlayerStats.Weather.Fog:
                GameObject.FindGameObjectWithTag("FogSystem").GetComponent<ParticleSystem>().Play();
                break;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void StopWeatherServerRPC() {
        StopWeatherClientRPC();
    }

    [ClientRpc]
    private void StopWeatherClientRPC() {
        // Stop the weather on the player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                // This check makes it so the king isn't affected
                if (player.GetComponentInChildren<PlayerStats>() != null) {
                    player.GetComponentInChildren<PlayerStats>().ClearWeather();
                }
            }
        }

        // Turn off the weather particle systems
        GameObject.FindGameObjectWithTag("RainSystem").GetComponent<ParticleSystem>().Stop();
        GameObject.FindGameObjectWithTag("SnowSystem").GetComponent<ParticleSystem>().Stop();
        GameObject.FindGameObjectWithTag("WindSystem").GetComponent<ParticleSystem>().Stop();
        GameObject.FindGameObjectWithTag("FogSystem").GetComponent<ParticleSystem>().Stop();
    }

    public void SpinWheel() {

        // Only spin if not spinning already and not on cooldown
        if (isSpinning || onCooldown) { return; }

        // Do the spin
        wheelSpeed = Random.Range(4.000f, 5.000f);
        subtractSpeed = 0.009f;
        isSpinning = true;
        onCooldown = true;

        // Also make the button uninteractable
        gameObject.GetComponent<Button>().interactable = false;
        pointer.GetComponent<Button>().interactable = false;
    }
}
