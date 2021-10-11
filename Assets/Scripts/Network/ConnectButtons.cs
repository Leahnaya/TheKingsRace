using MLAPI;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConnectButtons : MonoBehaviour {

    private NetworkManager netManager;

    public GameObject ErrorPanel;
    public Text ErrorText;

    public InputField ipAddressField;

    void Start() {
        // Make sure the Error Panel is not enabled to start
        ErrorPanel.SetActive(false);

        // Find the network manager that was created during the preloader scene
        netManager = FindObjectOfType<NetworkManager>();
    }

    // Connect to the dedicated server
    public void ConnectDedicatedServer () {
        // TODO: Connect to the dedicated server
        ThrowError("Connect to Dedicated Server Not Implemented Yet!");
    }

    // Host a private server
    public void HostPrivateServer() {
        // TODO: Host a private server
        ThrowError("Host Private Server Not Implemented Yet!");
    }

    // Connect to a private server via IP Address
    public void ConnectToPrivateServer() {
        string ipAddress = ipAddressField.text;

        // Check if the IP Address is valid
        if (!ValidateIPv4(ipAddress)) {
            ThrowError("Invalid IP Address!  Please enter a valid IP Address and try again!");
            return;
        }

        // IP Address is valid - Attempt to connect

        // TODO: Attempt to connect to host at ip address
        ThrowError("Connecting to Private Server Not Implemented Yet!\nValid IP Address: " + ipAddress);
    }

    private void ThrowError(string errorMsg) {
        ErrorText.text = errorMsg;

        ErrorPanel.SetActive(true);
    }

    public void DismissError() {
        ErrorText.text = "";

        ErrorPanel.SetActive(false);
    }

    // Method by: Habib (Stack Overflow)
    // https://stackoverflow.com/questions/11412956/what-is-the-best-way-of-validating-an-ip-address
    private bool ValidateIPv4(string ipString) {
        if (string.IsNullOrWhiteSpace(ipString)) {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4) {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }
}
