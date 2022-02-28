using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherWheel : MonoBehaviour {

    private float wheelSpeed;
    private float subtractSpeed;
    public bool isSpinning = false;

    void Update() {
        if (isSpinning) {
            transform.Rotate(0, 0, wheelSpeed);
            wheelSpeed -= subtractSpeed;
        }

        // Check when stopped spinning
        if (isSpinning && wheelSpeed <= 0) {
            wheelSpeed = 0;
            isSpinning = false;

            // Check the angle to see what won
            float finalAngle = transform.rotation.eulerAngles.z;
            
            if (finalAngle <= 45 || finalAngle > 315) {
                // Snow
                // TODO: Trigger Snow
                Debug.Log("Snow");

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            } else if (finalAngle > 45 && finalAngle <= 135) {
                // Windy
                // TODO: Trigger Wind
                Debug.Log("Windy");

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            } else if (finalAngle > 135 && finalAngle <= 225) {
                // Fog
                // TODO: Trigger Fog
                Debug.Log("Fog");

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 180);
            } else if (finalAngle > 225 && finalAngle <= 315) {
                // Rain
                // TODO: Trigger Rain
                Debug.Log("Rain");

                // Set angle back to centered
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
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
