using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    float startTIme;
    float duration = 1;
    // Start is called before the first frame update
    void Start()
    {
        startTIme = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTIme > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
