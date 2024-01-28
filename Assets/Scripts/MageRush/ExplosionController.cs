using Outloud.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    float startTIme;
    float duration = 1;

    void Start()
    {
        AudioManager.PlaySound("poks");
        startTIme = Time.time;

        if (NetworkManager.Singleton.IsServer)
        {
            var chars = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            foreach (var character in chars)
            {
                if ((character.transform.position - transform.position).magnitude < 2)
                    character.TakeDamage(1);
            }
        }
    }

    void Update()
    {
        if (Time.time - startTIme > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
