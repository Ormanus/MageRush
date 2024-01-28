using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationController : MonoBehaviour
{
    public SpriteAnimation frames;
    public int fps = 3;
    SpriteRenderer sr;
    float _startTime = 0f;

    public UnityEvent OnAnimationEnd = new();

    void Start()
    {
        if (!sr)
            sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeAnimation(SpriteAnimation newFrames, int newFps = 3)
    {
        frames = newFrames;
        fps = newFps;
        _startTime = Time.time;
    }

    public enum AnimationDirection
    {
        Left,
        Right
    }

    public void SetDirection(AnimationDirection direction)
    {
        if (!sr)
            sr = GetComponent<SpriteRenderer>();

        if (sr)
            sr.flipX = direction == AnimationDirection.Left;
    }

    void Update()
    {
        if (frames.sprites.Length > 0)
        {
            float t = (Time.time - _startTime);
            int frame = Mathf.FloorToInt(t * fps);
            sr.sprite = frames.sprites[frame % frames.sprites.Length];

            if (frame >= frames.sprites.Length)
            {
                OnAnimationEnd?.Invoke();
            }
        }
    }
}
