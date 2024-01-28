using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public SpriteAnimationSet[] spriteSets;
    CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        int playerCount = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None).Length;
        _characterController.spriteSet = spriteSets[(playerCount - 1) % spriteSets.Length];
        _characterController.player = playerCount - 1;
    }


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
            _characterController.MoveToDirection(dir);
        }
        else
        {
            _characterController.DoNotMove();
        }
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            // Attack
            _characterController.Attack();
        }

        if (Input.GetMouseButtonDown((int)MouseButton.Right))
        {
            // Attack
            _characterController.BigAttack();
        }
    }
}
