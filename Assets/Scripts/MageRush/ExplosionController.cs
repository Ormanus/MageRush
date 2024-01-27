using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    float startTIme;
    float duration = 1;

    void Start()
    {
        startTIme = Time.time;
    }

    void Update()
    {
        if (Time.time - startTIme > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
