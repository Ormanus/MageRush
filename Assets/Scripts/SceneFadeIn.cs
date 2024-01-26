using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Outloud.Common;

/// <summary>
/// Fades a graphic alpha from 1 -> 0
/// Made for fading a scene in
/// </summary>
[RequireComponent(typeof(Graphic))]
public class SceneFadeIn : MonoBehaviour
{
    public UnityEvent OnTransitionEnd;
    public float FadeDuration = 0.5f;
    Graphic _image;

    void Start()
    {
        _image = GetComponent<Graphic>();
        UIHelper.FadeAlpha(_image, 0f, UIHelper.Duration(FadeDuration), UIHelper.OnEnd(OnEnd));
    }

    void OnEnd()
    {
        OnTransitionEnd?.Invoke();
    }
}
