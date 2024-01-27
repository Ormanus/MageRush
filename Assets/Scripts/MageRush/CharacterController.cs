using System;
using System.Collections;
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

    public GameObject projectile;

    NetworkVariable<Vector2> Position = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<int> Health = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void TakeDamage(int amount)
    {
        if (IsServer)
        {
            Health.Value -= amount;
            if (Health.Value <= 0)
            {
                DieClientRpc();
            }
        }
    }

    Vector2 _spawnPoint;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _spawnPoint = transform.position;
            Health.OnValueChanged += UpdateHealth;
        }

        if (IsServer)
        {
            Health.Value = 10;
        }
    }

    private void UpdateHealth(int previousValue, int newValue)
    {
        if (newValue <= 0)
        {
            // Change health display
        }
    }

    public void Attack()
    {
        AttackServerRpc(transform.position);
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 pos)
    {
        CreateAttackClientRpc(pos);
    }

    [ClientRpc]
    public void CreateAttackClientRpc(Vector2 pos)
    {
        EffectFactory.Instance.CreateEffect(pos);
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

    [ClientRpc]
    public void DieClientRpc()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        float startTime = Time.time;
        Vector2 startPos = transform.position;
        while (Time.time - startTime < 3f)
        {
            float t = Time.time - startTime;
            transform.localEulerAngles = new Vector3(0f, 0f, t * 1000f);
            transform.position = (startPos + new Vector2(Mathf.Sin(t * 2f), Mathf.Cos(t * 2f)) * t * 10f);
            float s = 1f - (t / 4f);
            transform.localScale = Vector3.one * s;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        if (IsOwner)
            Position.Value = _spawnPoint;
        state = State.Idle;
    }

    void Die()
    {
        state = State.Dead;
    }
}
