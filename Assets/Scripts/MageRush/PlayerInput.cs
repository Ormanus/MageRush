using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (!GetComponent<NetworkObject>().IsOwner)
            return;

        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
        }
        if (dir != Vector2.zero)
        {
            GetComponent<CharacterController>().MoveToDirection(dir);
        }
        else
        {
            GetComponent<CharacterController>().DoNotMove();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Attack
            GetComponent<CharacterController>().Attack();
        }
    }
}
