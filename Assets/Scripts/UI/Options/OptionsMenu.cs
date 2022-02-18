using System.Collections;
using System.Collections.Generic;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    [SerializeField]
    private Toggle useMLAPIRelayToggle;

    void Start() {
        // Set the current values for options

        // ** Use MLAPI Relay
        if (PlayerPrefs.GetInt("UseMLAPIRelay") == 0) {
            useMLAPIRelayToggle.isOn = false;
        } else {
            useMLAPIRelayToggle.isOn = true;
        }

        // Add a listener to the toggle
        useMLAPIRelayToggle.onValueChanged.AddListener((isSelected) => {
            OnUseMLAPIRelayToggleChanged(isSelected);
        });

        // ** Other options here
    }

    private void OnUseMLAPIRelayToggleChanged(bool newState) {
        if (newState) {
            useMLAPIRelayToggle.isOn = true;
            PlayerPrefs.SetInt("UseMLAPIRelay", 1);

            // Don't forget to update the value on the network manager
            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<UNetTransport>().UseMLAPIRelay = true;
        } else {
            useMLAPIRelayToggle.isOn = false;
            PlayerPrefs.SetInt("UseMLAPIRelay", 0);

            // Don't forget to update the value on the network manager
            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<UNetTransport>().UseMLAPIRelay = false;
        }
    }

    public void OnReturnToTitleClicked() {
        SceneManager.LoadScene("TitleScene");
    }
}
