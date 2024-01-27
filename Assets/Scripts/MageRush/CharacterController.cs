using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Moving,
        Dead,
        Squished
    }

    public Sprite[] idleSprites;
    public Sprite[] movingSprites;
    public Sprite[] deadSprites;
    public Sprite[] squishedSprites;

    public State state = State.Idle;
    public float speed;

    public bool isHost;
    public bool isYou;
    public int hp = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void GetInput()
    {
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
            MoveToDirection(dir);
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isYou) {
            GetInput();
        }
    }

    void Idle()
    {
        state = State.Idle;
    }

    void MoveToDirection(Vector3 direction)
    {
        state = State.Moving;
        transform.position += direction.normalized * Time.deltaTime * speed;
    }

    void Die()
    {
        state = State.Dead;
    }
}
