using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public static float Fadetime = 0.25f;
    public Image TransparentBackground;
    public RectTransform OpaqueBackground;
    public bool UpAtStart = false;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    float _time = 0;
    Color _originalColor;
    Color _invisibleColor;
    public enum AnimationState
    {
        FadingIn,
        FadingOut,
        In,
        Out
    }
    [HideInInspector]
    AnimationState _animationState = AnimationState.Out;
    Vector2 _originalPosition;
    Vector2 _positionOutOfScreen;
    bool _started = false;

    void Awake()
    {
        if (_started)
            return;

        RectTransform SafeArea_ = GetComponent<RectTransform>();
        _originalPosition = OpaqueBackground.anchoredPosition;
        float heightOutOfScreen = -SafeArea_.rect.height*2 - OpaqueBackground.rect.height*2;

        _positionOutOfScreen = new Vector2(OpaqueBackground.anchoredPosition.x, heightOutOfScreen);

        _originalColor = TransparentBackground.color;
        _invisibleColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);

        if (!UpAtStart)
        {
            OpaqueBackground.anchoredPosition = _positionOutOfScreen;
            TransparentBackground.color = _invisibleColor;
            TransparentBackground.gameObject.SetActive(false);
            _animationState = AnimationState.Out;
            OnClose?.Invoke();
        }
        else
        {
            OpaqueBackground.anchoredPosition = _originalPosition;
            TransparentBackground.color = _originalColor;
            TransparentBackground.gameObject.SetActive(true);
            _animationState = AnimationState.In;
            OnOpen?.Invoke();
        }
        _started = true;
    }

    public AnimationState GetAnimationState()
    {
        return _animationState;
    }

    Vector3 GetPositionByFadeTime(float time)
    {
        float phase = time / Fadetime;
        if (_animationState == AnimationState.FadingOut)
        {
            return Vector3.Lerp(_positionOutOfScreen, _originalPosition, phase);
        }
        return Vector3.Lerp(_positionOutOfScreen, _originalPosition, 1.0f - Mathf.Pow(0.05f, phase));
    }
    
    void LateUpdate()
    {
        float dt = Time.deltaTime;
        switch (_animationState)
        {
            case AnimationState.FadingIn:
                _time += dt;
                if (_time > 10 * Fadetime)
                {
                    _animationState = AnimationState.In;
                }
                else
                {
                    OpaqueBackground.anchoredPosition = GetPositionByFadeTime(_time);
                    TransparentBackground.color = Color.Lerp(_invisibleColor, _originalColor, _time / Fadetime);
                }
                break;
            case AnimationState.FadingOut:
                _time -= dt;
                if (_time < 0)
                {
                    OpaqueBackground.anchoredPosition = _positionOutOfScreen;
                    _animationState = AnimationState.Out;
                    TransparentBackground.gameObject.SetActive(false);
                    OnClose?.Invoke();
                }
                else
                {
                    OpaqueBackground.anchoredPosition = GetPositionByFadeTime(_time);
                    TransparentBackground.color = Color.Lerp(_invisibleColor, _originalColor, _time / Fadetime);
                }
                break;
            default:
                break;
        }
    }

    public void Enable()
    {
        if (!_started)
            Awake();

        switch (_animationState)
        {
            case AnimationState.FadingOut:
            case AnimationState.Out:
                if (_time < 0)
                {
                    _time = 0;
                }
                _animationState = AnimationState.FadingIn;
                break;
            default:
                break;
        }
        TransparentBackground.gameObject.SetActive(true);
        OnOpen?.Invoke();
    }

    IEnumerator EnableDelay(float t)
    {
        yield return new WaitForSeconds(t);
        Enable();
    }

    public void EnableIn(float t)
    {
        StartCoroutine(EnableDelay(t));
    }

    public void Disable()
    {
        if (!_started)
            Awake();

        switch (_animationState)
        {
            case AnimationState.FadingIn:
            case AnimationState.In:
                if (_time > Fadetime)
                {
                    _time = Fadetime;
                }
                _animationState = AnimationState.FadingOut;
                break;
            default:
                break;
        }
    }
}
