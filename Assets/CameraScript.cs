using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject character;
    [SerializeField] [Range(0f, 1f)] float minMouseDisplacementRadius = .25f;
    [SerializeField] [Range(1f, 10f)] float displacementScale = 2f;

    public GameObject Character { set { character = value; } }

    void Start()
    {
        GetComponents();
    }

    void Update()
    {

        Vector3 target = new Vector3(character.transform.position.x,
                                     character.transform.position.y,
                                     transform.position.z);

        transform.position = target;
    }

    void GetComponents()
    {
        if (character == null)
        {
            character = GameObject.Find("PlayerChar");
        }
    }
}
