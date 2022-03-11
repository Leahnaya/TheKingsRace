using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySwap : MonoBehaviour
{
    public GameObject playerLobby;
    public GameObject kingLobby;

    public GameObject player;
    public GameObject king;

    // Start is called before the first frame update
    void Start()
    {
        kingLobby.SetActive(false);
        king.SetActive(false);
    }

    public void SwapLobby(){
        if(playerLobby.active){
            kingLobby.SetActive(true);
            king.SetActive(true);

            playerLobby.SetActive(false);
            player.SetActive(false);
        }
        else{
            playerLobby.SetActive(true);
            player.SetActive(true);

            kingLobby.SetActive(false);
            king.SetActive(false);
        }
    }
}
