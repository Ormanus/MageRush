using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public GameObject[] prefabs;

    public GameObject CreateEffect(Vector2 pos, string name)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var go = Instantiate(prefabs[i]);
                go.transform.position = pos;
                return go;
            }
        }
        return null;
    }
}
