using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour {

    [Header("Panels")]
    [SerializeField] private GameObject waitingForPlayerPanel;
    [SerializeField] private GameObject playerDataPanel;

    [Header("Data Display")]
    [SerializeField] private TMP_Text playerDisplayNameText;
    [SerializeField] private Image selectedCharacterImage;
    [SerializeField] private Toggle isReadyToggle;

    [Header("Images")]
    [SerializeField] private Sprite kingSprite;
    [SerializeField] private Sprite runnerSprite;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        playerDisplayNameText.text = lobbyPlayerState.PlayerName;
        isReadyToggle.isOn = lobbyPlayerState.IsReady;

        if (lobbyPlayerState.IsKing) {
            selectedCharacterImage.sprite = kingSprite;
        } else {
            selectedCharacterImage.sprite = runnerSprite;
        }

        waitingForPlayerPanel.SetActive(false);
        playerDataPanel.SetActive(true);
    }

    public void DisableDisplay()
    {
        waitingForPlayerPanel.SetActive(true);
        playerDataPanel.SetActive(false);
    }
}
