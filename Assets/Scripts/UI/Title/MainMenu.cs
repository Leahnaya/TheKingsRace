using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    public void OnOptionsClicked() {
        SceneManager.LoadScene("Options");
    }

    public void OnControlsClicked() {
        SceneManager.LoadScene("Controls");
    }

    public void OnQuitClicked() {
        Application.Quit();
    }

    public void OnCreditsClicked() {
        SceneManager.LoadScene("Credits");
    }
}
