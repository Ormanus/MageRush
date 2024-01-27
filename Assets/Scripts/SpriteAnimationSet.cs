using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SpriteAnimation
{
    public string name;
    public Sprite[] sprites;
    public float fps;
}

[CreateAssetMenu]
public class SpriteAnimationSet : ScriptableObject
{
    public SpriteAnimation[] spriteSets;
}
