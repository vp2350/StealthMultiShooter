using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public GameObject disconnectScreen;
    public GameObject disconnectCause;

    public GameObject loseScreen;
    public GameObject loseMessage;

    public GameObject winScreen;
    public GameObject winMessage;

    private int playersAtStart;

    // Start is called before the first frame update
    void Start()
    {
        // Record the number of players that are playing the game
        playersAtStart = PhotonNetwork.CurrentRoom.PlayerCount;

        // Disable players following the host to the main menu
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Set error message
        disconnectCause.GetComponent<TextMeshProUGUI>().text = cause.ToString();

        // Set disconnect screen as active
        disconnectScreen.SetActive(true);
    }

    /// <summary>
    /// Display the losing screen to the player
    /// Note: show the lose screen AFTER the player object has been destroyed
    /// </summary>
    public void ShowLoseScreen()
    {
        Debug.Log("ShowLoseScreen() called");
        // Only apply if the win screen has not already appeared (don't die to the border after winning)
        if(!winScreen.activeInHierarchy)
        {
            int survivors = GameObject.FindGameObjectsWithTag("Player").Length;
            loseMessage.GetComponent<TextMeshProUGUI>().text = $"Rank {survivors + 1} out of {playersAtStart}";
            loseScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Display the winning screen to the player
    /// </summary>
    public void ShowWinScreen()
    {
        Debug.Log("ShowWinScreen() called");

        // Only apply if the lose screen has not already appeared (don't win from the winner dying to the border)
        if (!loseScreen.activeInHierarchy)
        {
            winMessage.GetComponent<TextMeshProUGUI>().text = $"Rank 1 out of {playersAtStart}";
            winScreen.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
        
        // OnLeftRoom() will trigger the callback to return to the main menu
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene("MainMenu");
    }
}
