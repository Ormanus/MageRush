using Unity.Netcode;
using UnityEngine;

public class CharacterController : NetworkBehaviour
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

    public GameObject projectile;

    NetworkVariable<Vector2> Position = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {

        }
    }


    void Update()
    {
        transform.position = Position.Value;
    }

    void Idle()
    {
        state = State.Idle;
    }

    public void MoveToDirection(Vector2 direction)
    {
        state = State.Moving;
        Position.Value += direction.normalized * Time.deltaTime * speed;
    }

    public void MoveToPosition(Vector2 position)
    {
        transform.position = position;
    }

    void Die()
    {
        state = State.Dead;
    }

    public void Shoot()
    {
        Instantiate(projectile);
    }
}
