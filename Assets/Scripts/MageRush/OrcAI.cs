using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrcAI : MonoBehaviour
{
    public float seeDistance = 20;
    public CharacterController characterController;
    public float attackDistance = 1;
    public float attackRadius = 2;

    float seeDistanceSquared;
    float attackDistanceSquared;
    float attackRadiusSquared;

    float attackDuration = 1;
    float attackStartTime;

    // Start is called before the first frame update
    void Start()
    {
        seeDistanceSquared = seeDistance * seeDistance;
        attackDistanceSquared = attackDistance * attackDistance;
        attackRadiusSquared = attackRadius * attackRadius;
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
            if ((transform.position - closest.position).sqrMagnitude < attackDistanceSquared)
            {
                StartAttack();
            }
            else if ((transform.position - closest.position).sqrMagnitude < seeDistanceSquared)
            {
                characterController.MoveToDirection(closest.position - transform.position);
            }
            else
            {
                characterController.Idle();
            }
        }
        else
        {
            characterController.Idle();
        }
    }

    void StartAttack()
    {
        characterController.AttackState();
        attackStartTime = Time.time;
        Attack();
    }

    void Attack()
    {
        if (Time.time > attackStartTime + attackDuration)
        {
            var chars = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            foreach (var character in chars)
            {
                if (character == characterController)
                    continue;

                if ((character.transform.position - transform.position).sqrMagnitude < attackRadiusSquared)
                    character.TakeDamage(1);
            }
            characterController.Idle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (characterController.AnimationState)
        {
            case CharacterController.State.Moving:
                Move();
                break;
            case CharacterController.State.Attacking:
                Attack();
                break;
            default:
                Move();
                break;
        }
    }


}
