using System.Collections.Generic;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour {

    void Start() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        if (Application.isEditor) {
            // Swap to Title Screen (as client) while in the editor
            SceneManager.LoadScene(1);
        }

        var args = GetCommandlineArgs();

        // Check for the dedicated server command flags
        if (args.TryGetValue("-mlapi", out string mlapiValue)) { 
            if (mlapiValue == "server") {
                // Start the server
                NetworkManager.Singleton.StartServer();

                // Swap to the lobby scene to await players
                NetworkSceneManager.SwitchScene("Lobby");
            } 
        } else {
            // If not command line arguments
            // Swap to Title Screen (as client)
            SceneManager.LoadScene(1);
        }
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback) {

        bool approve = false;

        // Validate the connection password
        string password = System.Text.Encoding.ASCII.GetString(connectionData);
        if (password == "kingsrace") {
            approve = true;
        }

        // Vec 3 is position
        callback(true, null, approve, new Vector3(0, 10, 0), Quaternion.identity);
    }

    private Dictionary<string, string> GetCommandlineArgs() {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i) {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-")) {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}
