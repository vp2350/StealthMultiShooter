using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Spawns in the player at their spawn point
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    //Anything that shows up after "Resources/"
    private const string PREFAB_LOCATION = "Player";

    /// <summary>
    /// Spawn in the player at their spawn point
    /// </summary>
    private void Start()
    {
        int index = -1;
        Player[] players = PhotonNetwork.PlayerList;
        //Determine which player index we are
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i] == PhotonNetwork.LocalPlayer)
            {
                index = i;
                break;
            }
        }
        //Get the spawn point relative to our index, or the origin of the world if we don't
        //have enough spawn points
        Transform[] spawnPoints = SpawnPointManager.Instance.SpawnPoints;
        Vector3 spawnPosition = (index >= spawnPoints.Length || index< 0)
            ? Vector3.zero : spawnPoints[index].position;
        //Spawn the player in at the spawn point
        GameObject player = 
            PhotonNetwork.Instantiate(PREFAB_LOCATION, spawnPosition, Quaternion.identity);
        Camera.main.GetComponent<CameraScript>().Character = player;
    }
}
