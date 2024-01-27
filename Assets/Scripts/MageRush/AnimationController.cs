using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Sprite[] frames;
    public int fps = 3;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeAnimation(Sprite[] newFrames, int newFps = 3)
    {
        frames = newFrames;
        fps = newFps;
    }

    // Update is called once per frame
    void Update()
    {
        if (frames.Length > 0)
        {
            sr.sprite = frames[Mathf.FloorToInt(Time.time * fps) % frames.Length];
        }
    }
}
