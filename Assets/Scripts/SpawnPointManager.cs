using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a set of spawn points that can be referenced by the player
/// </summary>
public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance;
    //Drop all your spawn points in here
    [SerializeField]
    private Transform[] spawnPoints = new Transform[0];

    /// <summary>
    /// Basic singleton setup for spawnpoints
    /// </summary>
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Property to get the spawn points
    /// </summary>
    public Transform[] SpawnPoints { get { return spawnPoints; } }
}
