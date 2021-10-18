using System.Collections.Generic;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour {

    void Start() {
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
