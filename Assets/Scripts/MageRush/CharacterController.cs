using Outloud.Common;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : NetworkBehaviour
{
    public enum State
    {
        Idle,
        Moving,
        Dead,
        Splat,
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

    public int maxHealth = 10;
    public float speed;
    public int player = -1;

    public Shield shield;

    //NetworkVariable<Vector2> Position = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<int> Health = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    static CharacterController ServerController = null;

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

            if (TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.color = Color.red;
            }
            Timing.DoAfter(NormalColor, 0.2f);

            Health.Value -= amount;
            if (Health.Value <= 0)
            {
                DieClientRpc();
                Health.Value = maxHealth;
            }
        }
    }

    void NormalColor()
    {
        if (TryGetComponent<SpriteRenderer>(out var sr))
            {
            sr.color = Color.white;
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

    public static void RequestEffect(Vector2 posiiton, string effectName, Vector2 direction)
    {
        if (ServerController != null)
        {
            ServerController.CreateEffectClientRpc(posiiton, effectName, direction);
        }
    }

    [ClientRpc]
    public void CreateEffectClientRpc(Vector2 posiiton, string effectName, Vector2 direction)
    {
        EffectFactory.Instance.CreateEffect(posiiton, effectName, direction);
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
            Health.Value = maxHealth;

            if (IsOwner)
            {
                ServerController = this;
            }
        }
    }

    private void Awake()
    {
        //Position.Value = transform.position;
    }

    private void UpdateHealth(int previousValue, int newValue)
    {
        if (newValue <= 0)
        {
            // Change health display
        }
    }

    public void Squish()
    {
        DoNotMove();
        AnimationState = State.Splat;
        animationController.OnAnimationEnd.AddListener(() => { animationController.OnAnimationEnd.RemoveAllListeners(); Idle(); });
    }

    public void Attack()
    {
        if (AnimationState == State.Splat){ return; }
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mousePosition - transform.position;
        animationController.SetDirection(delta.x < 0 ? AnimationController.AnimationDirection.Left : AnimationController.AnimationDirection.Right);
        AttackServerRpc((Vector2)transform.position + delta.normalized, delta);
    }

    public void BigAttack()
    {
        if (AnimationState == State.Splat) { return; }
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 delta = mousePosition - transform.position;
        animationController.SetDirection(delta.x < 0 ? AnimationController.AnimationDirection.Left : AnimationController.AnimationDirection.Right);
        BigAttackServerRpc((Vector2)transform.position + delta.normalized, delta);
    }

    [ServerRpc]
    public void AttackServerRpc(Vector2 pos, Vector2 dir)
    {
        CreateAttackClientRpc(pos, dir);
    }

    [ServerRpc]
    public void BigAttackServerRpc(Vector2 pos, Vector2 dir)
    {
        CreateBigAttackClientRpc(pos, dir);
    }

    [ClientRpc]
    public void CreateAttackClientRpc(Vector2 pos, Vector2 dir)
    {
        EffectFactory.Instance.CreateProjectile(this, pos, dir, GetAttackName(false));
        AttackState();
    }

    [ClientRpc]
    public void CreateBigAttackClientRpc(Vector2 pos, Vector2 dir)
    {
        EffectFactory.Instance.CreateProjectile(this, pos, dir, GetAttackName(true));
        AttackState();
    }

    string GetAttackName(bool big)
    {
        string[] smallAttacks =
        {
            "firewhip",
            "bubble",
            "dirtball",
            "arrow"
        };
        string[] bigAttacks =
{
            "fireball",
            "wave",
            "boulder",
            "shield"
        };
        if (player == -1)
            return "BOOM";

        return big ? bigAttacks[player] : smallAttacks[player];
    }

    public void Idle()
    {
        AnimationState = State.Idle;
    }

    public void DoNotMove()
    {
        DoNotMoveServerRpc();
        if (AnimationState == State.Moving)
        {
            Idle();
        }
    }

    [ServerRpc]
    public void DoNotMoveServerRpc()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void AttackState()
    {
        AnimationState = State.Attacking;
        animationController.OnAnimationEnd.AddListener(AttackFinished);
    }

    public void AttackStateCustomEnd(UnityAction unityAction)
    {
        AnimationState = State.Attacking;
        animationController.OnAnimationEnd.AddListener(() => { animationController.OnAnimationEnd.RemoveAllListeners(); unityAction(); });
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
            MoveServerRpc(direction);
        }
    }

    [ServerRpc]
    public void MoveServerRpc(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    [ServerRpc]
    public void TeleportServerRpc(Vector2 position)
    {
        GetComponent<Rigidbody2D>().position = position;
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
            // TODO: checkpoint system!
            _spawnPoint = Vector2.zero;
            TeleportServerRpc(_spawnPoint);
        }
        AnimationState = State.Idle;
    }

    void Die()
    {
        AnimationState = State.Dead;
    }
}
