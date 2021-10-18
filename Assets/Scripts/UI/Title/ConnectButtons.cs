using MLAPI;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectButtons : MonoBehaviour {

    // The IP Used to connect to the dedicated server
    const string DEDICATED_SERVER_IP = "127.0.0.1";
    const int DEDICATED_SERVER_PORT = 7777;

    public GameObject ErrorPanel;
    public Text ErrorText;

    public InputField ipAddressField;
    public TMP_InputField playerNameField;

    private int connectionTimeoutTime = 5;

    void Start() {
        // Make sure the Error Panel is not enabled to start
        ErrorPanel.SetActive(false);

        PlayerPrefs.GetString("PlayerName");
    }

    // Connect to the dedicated server
    public void ConnectDedicatedServer () {
        // Throw an error for now
        ThrowError("Not implemented yet.");
    }

    // Host a private server
    public void HostPrivateServer() {
        // Set the players name
        PlayerPrefs.SetString("PlayerName", playerNameField.text);

        // Start a server and join (hosting)
        GameNetPortal.Instance.StartHost();
    }

    // Connect to a private server via IP Address
    public void ConnectToPrivateServer() {
        string ipAddress = ipAddressField.text;
        int port = 7777;

        // Check if the IP Address is valid
        if (!ValidateIPv4(ipAddress)) {
            ThrowError("Invalid IP Address!  Please enter a valid IP Address and try again!");
            return;
        }

        // IP Address is valid - Attempt to connect

        // Set the players name
        PlayerPrefs.SetString("PlayerName", playerNameField.text);

        // Start the game client
        ClientGameNetPortal.Instance.StartClient(ipAddress, port);

        // Run a coroutine to check if the client connects to the server
        StartCoroutine(checkIsConnectedClient());
    }

    IEnumerator checkIsConnectedClient() {
        yield return new WaitForSecondsRealtime(connectionTimeoutTime);

        if (!NetworkManager.Singleton.IsConnectedClient) {
            // Failed to connect to the server
            NetworkManager.Singleton.StopClient();

            ThrowError("Could not connect to server!\nReason: Connection Timed Out");
        }
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
