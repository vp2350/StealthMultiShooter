using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public Scene PlayButtonTrigger;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject loadingScreen;

    public void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom("default", new RoomOptions { MaxPlayers = 16 }, null);
    }

    public override void OnJoinedRoom()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
    }

    public void PlayGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            SceneManager.LoadScene("Gameplay_Kevin");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
