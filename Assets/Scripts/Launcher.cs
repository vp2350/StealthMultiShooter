using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject loadingScreen;
    public GameObject playerCount;

    public GameObject disconnectScreen;
    public GameObject disconnectCause;

    [SerializeField]
    private GameObject playButton;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        timer = 0;
    }

    void Update()
    {
        //timer += Time.deltaTime;
        //if(timer >= 1f)
        //{
        //    timer -= 1f;
        //    UpdateLobbyPlayerCount();
        //}
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        UpdateLobbyPlayerCount();
        Debug.Log("Connected successfully!");
        playButton.SetActive(true);
        //PhotonNetwork.JoinOrCreateRoom("default", new RoomOptions { MaxPlayers = 16 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room!");
        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joining room failed (return code: {returnCode}, message: {message}");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // UpdateLobbyPlayerCount();
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // UpdateLobbyPlayerCount();
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Set error message
        disconnectCause.GetComponent<TextMeshProUGUI>().text = cause.ToString();

        // Set disconnect screen as active
        disconnectScreen.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateLobbyPlayerCount()
    {
        // playerCount.GetComponent<TextMeshProUGUI>().SetText($"({PhotonNetwork.CountOfPlayers}/{PhotonNetwork.CountOfPlayersOnMaster})");
    }
}
