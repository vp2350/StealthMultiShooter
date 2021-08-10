using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject[] lightmasks;

    [SerializeField] [Range(5, 50)] float maxWalkSpeed = 150f;
    [SerializeField] bool accelerationEnabled = true;
    [SerializeField] [Range(1, 200)] float accelerationpercent = 110f;
    float acceleration, speed;
    Vector2 move;
    private PhotonView photonView;
    int stickCount = 2;
    //Combat
    [SerializeField] private float health;
    private float maxHealth;
    public GameObject healthBar;
    [SerializeField] public GameObject bullet;
    public GameObject glowStick;

    // Border
    private Collider2D thisCollider;
    private Collider2D borderCollider;
    private float outsideBorderTimer = 0;

    public GameObject shootPoint;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        // Set the health since all players have access to health info
        health = 100;
        maxHealth = 100;
        
        if (!photonView.IsMine)
        {
            foreach(GameObject lightmask in lightmasks)
            {
                lightmask.SetActive(false);
            }
            return;
        }
        acceleration = (accelerationpercent / 100f) * maxWalkSpeed;

        stickCount = 2;
        thisCollider = GetComponent<BoxCollider2D>();
        borderCollider = GameObject.Find("BorderCircle").GetComponent<CircleCollider2D>();
        GetComponents();
    }

    void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        RotateSprite();
        UpdateMoveControls();
        FireWeapon();
        ThrowStick();
        CheckBorder();
    }

    private void FixedUpdate()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        UpdateMove();
    }

    #region updates

    void FireWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject bullet1 = PhotonNetwork.Instantiate("Bullet", shootPoint.transform.position, shootPoint.transform.rotation);
            bullet1.GetComponent<BulletScript>().Init(shootPoint.transform.up);
        }
    }

    void ThrowStick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && stickCount>0)
        {
            GameObject stick1 = PhotonNetwork.Instantiate("Glowstick", shootPoint.transform.position, shootPoint.transform.rotation);
            stick1.GetComponent<Rigidbody2D>().AddForce(shootPoint.transform.up * 10, ForceMode2D.Impulse);
            stickCount--;
        }
    }

    /// <summary>
    /// Check to determine if the player is colliding with the border
    /// If so, increment the timer, and take 10 damage for every second outside the border
    /// </summary>
    void CheckBorder()
    {
        if(thisCollider.IsTouching(borderCollider))
        {
            outsideBorderTimer = 0;
        } 
        else
        {
            // Player is fully outside of the border
            outsideBorderTimer += Time.deltaTime;
            if(outsideBorderTimer >= 1f)
            {
                // Take 10 damage per second from the border
                outsideBorderTimer -= 1f;
                GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 10f);
            }
        }
    }

    void RotateSprite()
    {
        // Player can control direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //get mouse position

        Vector3 direction = transform.position - mousePos; //get vector from position to mouse pos
        float angleOfRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90; //get angle
        transform.localRotation = Quaternion.Euler(0, 0, angleOfRotation); //set rotation
    }

    void UpdateMove()
    {


        if (accelerationEnabled)
        { //accelearte if enabled
            if (move.magnitude > 0) speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxWalkSpeed);
            else speed = Mathf.Max(speed - acceleration * Time.deltaTime, 0f);
        }
        else
        { //otherwize go to max speed
            speed = maxWalkSpeed;
        }

        move = move * speed * Time.deltaTime; //multiply by speed and time
        Vector2 target = new Vector2(transform.position.x + move.x, transform.position.y + move.y); //add to move
        rb.MovePosition(target); //move rigidbody
    }

    void UpdateMoveControls()
    {
        float x = 0, y = 0;
        if (Input.GetKey(KeyCode.A))
        {
            x = -4;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x = 4;
        }

        if (Input.GetKey(KeyCode.W))
        {
            y = 4;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y = -4;
        }

        move = new Vector2(x, y); //turn into vector
        move = move.normalized; //normalize
    }
    #endregion

    void GetComponents()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (bullet == null)
        {
            bullet = (GameObject)Resources.Load("prefabs/bullet", typeof(GameObject));
        }

        if (glowStick == null)
        {
            glowStick = (GameObject)Resources.Load("prefabs/Glowstick", typeof(GameObject));
        }
    }

    [PunRPC]
    public void TakeDamage(float attackDamage)
    {
        health -= attackDamage;

        if (health <= 0)
            KillPlayer();
        else
            UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.GetComponent<UnityEngine.UI.Image>().fillAmount = health / maxHealth;
    }

    /// <summary>
    /// When this player's health reaches 0, destroy the player and trigger the end game panel
    /// </summary>
    public void KillPlayer()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            GameObject.Find("GameStateManager").GetComponent<GameStateManager>().ShowLoseScreen();
        }
        else
        {
            // Not the client's player; are they the last one standing?
            // Count both the client player and the player that is actively dying
            if(GameObject.FindGameObjectsWithTag("Player").Length == 2)
            {
                GameObject.Find("GameStateManager").GetComponent<GameStateManager>().ShowWinScreen();
            }
        }
    }

    /// <summary>
    /// Whenever a player dies, check if there is only one player remaining
    /// </summary>
    public void OnDestroy()
    {
        int counter = 0;
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject obj in objects)
        {
            counter++;
        }

        Debug.Log($"Players remaining: {counter}");
    }
}
