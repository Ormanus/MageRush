using Outloud.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrollAI : MonoBehaviour
{
    public float seeDistance = 20;
    public CharacterController characterController;
    public float attackDistance = 1;
    public float attackRadius = 2;
    public float minAttackInterval = 3;

    float seeDistanceSquared;
    float attackDistanceSquared;
    float attackRadiusSquared;

    float lastAttackTime;

    // Start is called before the first frame update
    void Start()
    {
        seeDistanceSquared = seeDistance * seeDistance;
        attackDistanceSquared = attackDistance * attackDistance;
        attackRadiusSquared = attackRadius * attackRadius;
        lastAttackTime = -minAttackInterval;
    }

    void Move()
    {
        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        if (players.Length > 0)
        {
            Transform closest = players[0].transform;
            for (int i = 1; i < players.Length; i++)
            {
                Transform candidate = players[i].transform;
                if ((transform.position - candidate.position).sqrMagnitude < (transform.position - closest.position).sqrMagnitude)
                {
                    closest = candidate;
                }
            }
            if ((transform.position - closest.position).sqrMagnitude < attackDistanceSquared && Time.time > lastAttackTime + minAttackInterval)
            {
                StartAttack();
            }
            else if ((transform.position - closest.position).sqrMagnitude < seeDistanceSquared)
            {
                characterController.MoveToDirection(closest.position - transform.position);
            }
            else
            {
                characterController.DoNotMove();
                characterController.Idle();
            }
        }
        else
        {
            characterController.DoNotMove();
            characterController.Idle();
        }
    }

    void StartAttack()
    {
        characterController.DoNotMove();
        characterController.AttackStateCustomEnd(EndAttack);
        lastAttackTime = Time.time;
    }

    void EndAttack()
    {
        Debug.Log("WTF");
        var chars = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
        foreach (var character in chars)
        {
            if (character == characterController)
                continue;

            if ((character.transform.position - transform.position).sqrMagnitude < attackRadiusSquared)
            {
                AudioManager.PlaySound("thump");
                character.Squish();
                character.TakeDamage(1);
            }
        }
        characterController.Idle();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!characterController.IsServer)
            return;

        switch (characterController.AnimationState)
        {
            case CharacterController.State.Moving:
            case CharacterController.State.Idle:
                Move();
                break;
            case CharacterController.State.Attacking:
            default:
                break;
        }
    }


}
