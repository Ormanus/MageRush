using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    public SpriteAnimation frames;
    public int fps = 3;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeAnimation(SpriteAnimation newFrames, int newFps = 3)
    {
        frames = newFrames;
        fps = newFps;
    }


    void Update()
    {
        if (frames.sprites.Length > 0)
        {
            sr.sprite = frames.sprites[Mathf.FloorToInt(Time.time * fps) % frames.sprites.Length];
        }
    }
}
