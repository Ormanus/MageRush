using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackScriptableObject", order = 1)]
public class AttackScriptableObject : ScriptableObject
{
    [Serializable]
    public class Attack
    {
        public GameObject prefab;
        public bool largeAttack;
        public Projectile.Element primaryElement;
        public Projectile.Element secondaryElement;
    }

    public List<Attack> attacks;
}