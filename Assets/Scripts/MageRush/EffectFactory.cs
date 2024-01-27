using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public GameObject prefab;

    public void CreateEffect(Vector2 pos)
    {
        var go = Instantiate(prefab);
        go.transform.position = pos;
    }
}
