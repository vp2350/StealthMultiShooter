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

    [SerializeField]
    private GameObject playButton;

    // Start is called before the first frame update
    void Start()
    {
        // Synchronize moving from the lobby room to in-game
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            // Already connected to a lobby
            // Enable the play button and leave the last game's room
            playButton.SetActive(true);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected successfully!");
        playButton.SetActive(true);
        //PhotonNetwork.JoinOrCreateRoom("default", new RoomOptions { MaxPlayers = 16 }, null);
    }

    /// <summary>
    /// Creates custom room property of a random seed when the room is first created
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["RandomSeed"] = (int)System.DateTime.Now.Ticks;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room!");
        base.OnJoinedRoom();
        UpdateLobbyPlayerCount();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Joining room failed (return code: {returnCode}, message: {message}");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdateLobbyPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdateLobbyPlayerCount();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    private void UpdateLobbyPlayerCount()
    {
        playerCount.GetComponent<TextMeshProUGUI>().SetText($"Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }
}
