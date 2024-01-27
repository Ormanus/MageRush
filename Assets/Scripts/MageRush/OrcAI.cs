using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrcAI : MonoBehaviour
{
    public float seeDistance = 20;
    public CharacterController characterController;

    float seeDistanceSquared;
    // Start is called before the first frame update
    void Start()
    {
        seeDistanceSquared = seeDistance * seeDistance; 
    }

    // Update is called once per frame
    void Update()
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

            if ((transform.position - closest.position).sqrMagnitude < seeDistanceSquared)
            {
                characterController.MoveToDirection(closest.position - transform.position);
            }
        }
    }


}
