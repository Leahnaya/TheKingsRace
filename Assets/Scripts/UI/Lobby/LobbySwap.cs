using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySwap : MonoBehaviour
{
    GameObject playerLobby;
    GameObject kingLobby;

    // Start is called before the first frame update
    void Start()
    {
        playerLobby = GameObject.Find("PlayerLobby");
        kingLobby = GameObject.Find("KingLobby");

        kingLobby.SetActive(false);
    }

    public void SwapLobby(){
        if(playerLobby.active){
            kingLobby.SetActive(true);
            playerLobby.SetActive(false);
        }
        else{
            playerLobby.SetActive(true);
            kingLobby.SetActive(false);
        }
    }
}
