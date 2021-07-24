using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed;
    public float damage;

    private Rigidbody2D myRigdBody;
    private bool canDamage; //Avoids repeat hits
    private Vector3 forwardVector;

    // Start is called before the first frame update
    void Start()
    {
        canDamage = true;
        myRigdBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myRigdBody.MovePosition(transform.position + forwardVector * Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canDamage)
        {
            canDamage = false; 
            if(collision.collider.tag == "Player")
            {
                collision.collider.gameObject.GetComponent<PlayerMove>().TakeDamage(damage);
            }
        }
        Destroy(this.gameObject);
    }

    public void Init(Vector3 forward)
    {
        forwardVector = forward;
    }
}
