using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory : Singleton<EffectFactory>
{
    public GameObject[] prefabs;

    public GameObject CreateEffect(Vector2 pos, string name, Vector2 direction = default)
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

    public GameObject CreateProjectile(CharacterController owner, Vector2 pos, Vector2 direction, string name, float? speed = null)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                var go = Instantiate(prefabs[i]);
                go.transform.position = pos;
                Projectile proj = go.GetComponent<Projectile>();
                proj.owner = owner;
                proj.direction = direction.normalized;
                if (speed != null)
                    proj.speed = speed.Value;

                return go;
            }
        }
        return null;
    }
}
