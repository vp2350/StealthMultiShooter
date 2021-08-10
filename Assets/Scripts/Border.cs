using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Border : MonoBehaviour
{
    // Background object for the playing map
    public GameObject map;
    private Vector3 mapCenter;
    private float mapWidth;
    private float mapHeight;

    // Reference to border circle
    public GameObject borderCircle;

    // Timer calculation values
    public float pauseDuration = 20;
    public float pauseTimer = 0;
    public float moveDuration = 20;
    public float moveTimer = 0;
    private Vector2 startPos;
    private Vector2 endPos;
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        // Get initial map sizing data from the background component
        mapCenter = map.transform.position;
        mapWidth = map.GetComponent<SpriteRenderer>().bounds.size.x;
        mapHeight = map.GetComponent<SpriteRenderer>().bounds.size.y;

        // Calculate/set the starting border values to be centered, and outside the map
        transform.position = mapCenter;
        startPos = new Vector2(0, 0);
        startScale = Mathf.Max(mapWidth, mapHeight) *1.2f;
        borderCircle.transform.localPosition = startPos;
        borderCircle.transform.localScale = new Vector2(startScale, startScale);

        // Set up the timer state
        pauseTimer = pauseDuration;
        moveTimer = 0;

        //Set up the random
        //Important to make sure that they are used exactly the same amount of times in the same order to prevent desync
        random = new System.Random((int)PhotonNetwork.CurrentRoom.CustomProperties["RandomSeed"]);
    }

    // Update is called once per frame
    void Update()
    {
        // Timer > 0 signifies an active phase
        if(pauseTimer > 0)
        {
            // Pause phase
            UpdatePausePhase();
        } 
        else
        {
            // Move phase
            UpdateMovePhase();
        }
    }

    private void BeginPausePhase()
    {
        // Update the timer values
        pauseTimer = pauseDuration;
        moveTimer = 0;
    }

    private void UpdatePausePhase()
    {
        // Nothing active (no UI implemented)

        // Update the timer
        pauseTimer -= Time.deltaTime;

        // If the phase ends, prepare swapping phases for the next frame
        if (pauseTimer <= 0)
        {
            BeginMovePhase();
        }
    }

    private void BeginMovePhase()
    {
        // Save the current location/scale
        startPos = borderCircle.transform.position;
        startScale = borderCircle.transform.localScale.x;

        // Calculate the target location/scale
        endPos = new Vector2(
            borderCircle.transform.position.x + (((float)random.NextDouble() * 0.5f) - 0.25f) * startScale,
            borderCircle.transform.position.y + (((float)random.NextDouble() * 0.5f) - 0.25f) * startScale);
        endScale = (startScale * (((float)random.NextDouble() * 0.25f) + 0.5f) - 1f);

        // Update the timer values
        pauseTimer = 0;
        moveTimer = moveDuration;
    }

    private void UpdateMovePhase()
    {
        // Apply movement to the circle
        borderCircle.transform.position = Vector2.Lerp(startPos, endPos, 1f - (moveTimer / moveDuration));
        float currentScale = Mathf.Lerp(startScale, endScale, 1f - (moveTimer / moveDuration));

        // Check to ensure the border doesn't invert
        if(currentScale <= 0)
        {
            // Move the circle off-screen
            borderCircle.transform.position = new Vector2(100000f, 100000f);

            // Wait indefinitely
            BeginPausePhase();
            pauseDuration = 3600;
        }

        borderCircle.transform.localScale = 
            new Vector3(1f, 1f, 1f) * currentScale;
        
        // Update the timer for the next frame
        moveTimer -= Time.deltaTime;

        // If the phase ends, prepare swapping phases for the next frame
        if (moveTimer <= 0)
        {
            BeginPausePhase();
        }
    }
}
