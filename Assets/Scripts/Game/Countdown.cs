using System.Collections;
using System.Collections.Generic;
using TMPro;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Countdown : NetworkBehaviour {

    public TMP_Text countdown_text;
    private GameObject PauseMenu;

    void Start() {
        countdown_text.text = "Ready?";
        StartCoroutine(BeginCoundown());
        PauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
    }


    IEnumerator BeginCoundown() {
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "Go!";
        if (IsHost){
            EnablePauseMenuServerRPC();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void EnablePauseMenuServerRPC(){

        //allows pause menu to be activated of 3 2 1 countdown.
        EnablePauseMenuClientRPC();
    }
    [ClientRpc]
    private void EnablePauseMenuClientRPC(){
        PauseMenu.GetComponent<PauseMenu>().isUsable = true;
    }
}
