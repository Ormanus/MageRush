using System;
using System.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class CharacterController : NetworkBehaviour
{
    public enum State
    {
        Idle,
        Moving,
        Dead,
        Squished,
        Attacking
    }

    public SpriteAnimationSet spriteSet;
    public AnimationController animationController;

    State _state = State.Idle;
    public State AnimationState
    {
        get { return _state; }
        set 
        {
            if (_state != value)
            {
                Debug.Log("New state: " + value);
                SelectAnimationSet(value);
            }
            _state = value; 
        }
    }

    public float speed;

    public Shield shield;

    public GameObject projectile;

    NetworkVariable<Vector2> Position = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<int> Health = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void TakeDamage(int amount)
    {
        if (IsServer)
        {
            if (shield != null)
            {
                shield.hitpoints -= amount;
                if (shield.hitpoints <= 0)
                {
                    Destroy(shield);
                    shield = null;
                }
            }

            Health.Value -= amount;
            if (Health.Value <= 0)
            {
                DieClientRpc();
            }
        }
    }

    [ClientRpc]
    public void GainShieldClientRpc()
    {

    }

    [ClientRpc]
    public void LoseShieldClientRpc()
    {

    }

    Vector2 _spawnPoint;

    void SelectAnimationSet(State newState)
    {
        if (spriteSet == null)
            return;

        foreach (var set in spriteSet.spriteSets)
        {
            string setName = Enum.GetName(typeof(State), newState);
            if (set.name.Equals(setName, StringComparison.InvariantCultureIgnoreCase))
            {
                animationController.ChangeAnimation(set);
                return;
            }
        }
        Debug.Log("No matching animation set found!");
    }

    public override void OnNetworkSpawn()
    {
        AnimationState = State.Idle;
        SelectAnimationSet(State.Idle);

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

    private void Awake()
    {
        Position.Value = transform.position;
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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mousePosition - transform.position;
        animationController.SetDirection(delta.x < 0 ? AnimationController.AnimationDirection.Left : AnimationController.AnimationDirection.Right);
        AttackServerRpc(transform.position, delta);
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 pos, Vector2 dir)
    {
        CreateAttackClientRpc(pos, dir);
    }

    [ClientRpc]
    public void CreateAttackClientRpc(Vector2 pos, Vector2 dir)
    {
        EffectFactory.Instance.CreateProjectile(pos, dir, "fireball");
        AttackState();
    }

    public void Idle()
    {
        AnimationState = State.Idle;
    }

    public void DoNotMove()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (AnimationState == State.Moving)
        {
            Idle();
        }
    }

    public void AttackState()
    {
        AnimationState = State.Attacking;
        animationController.OnAnimationEnd.AddListener(AttackFinished);
    }

    void AttackFinished()
    {
        animationController.OnAnimationEnd.RemoveAllListeners();
        AnimationState = State.Idle;
    }

    public void MoveToDirection(Vector2 direction)
    {
        if (AnimationState == State.Idle || AnimationState == State.Moving)
        {
            if (direction.x < 0)
            {
                animationController.SetDirection(AnimationController.AnimationDirection.Left);
            }
            else if (direction.x > 0)
            {
                animationController.SetDirection(AnimationController.AnimationDirection.Right);
            }
            AnimationState = State.Moving;
            GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
        }
    }

    public void MoveToPosition(Vector2 position)
    {
        AnimationState = State.Moving;
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
            transform.localEulerAngles = new Vector3(0f, 0f, t * 2000f);
            transform.position = startPos + (new Vector2(Mathf.Sin(t * 2f), Mathf.Cos(t * 2f)) * t * 8f);
            float s = 1f - (t / 4f);
            transform.localScale = Vector3.one * s;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        if (IsOwner)
        {
            GetComponent<Rigidbody2D>().position = _spawnPoint;
            Position.Value = _spawnPoint;
        }
        AnimationState = State.Idle;
    }

    void Die()
    {
        AnimationState = State.Dead;
    }
}
