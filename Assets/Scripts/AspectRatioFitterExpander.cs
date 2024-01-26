using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(AspectRatioFitter))]
public class AspectRatioFitterExpander : MonoBehaviour
{
    public float MinHeight;
    public float MinWidth;

    RectTransform _rt;

    AspectRatioFitter _fitter;

    void Start()
    {
        _fitter = GetComponent<AspectRatioFitter>();
        _rt = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        if (_rt.rect.width < MinWidth)
        {
            _fitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            _rt.sizeDelta = new Vector2(MinWidth, 0);
        }

        if (_rt.rect.height < MinWidth)
        {
            _fitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            _rt.sizeDelta = new Vector2(0, MinHeight);
        }
    }
}
