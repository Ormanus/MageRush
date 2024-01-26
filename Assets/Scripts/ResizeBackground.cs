using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ResizeBackground : MonoBehaviour {

    public enum VerticalAlignment
    {
        Up,
        Cemter,
        Down
    }

    [Header("Resize dimensions:")]
    public bool X;
    public bool Y;
    [Space]
    public VerticalAlignment Alignment;

    private SpriteRenderer _sr;
	private int _height, _width = 0; // Screen dimensions
    private float _aspectRatio; // Sprite's aspect ratio
    
	void Awake () 
	{
		_sr = GetComponent<SpriteRenderer>();
	}
	
	void Update () 
	{
        if(Screen.width != _width || Screen.height != _height)
			Resize();
	}

	void Resize()
	{
		if(_sr == null) return;

        transform.localScale = Vector3.one;

		float width  = _sr.sprite.bounds.size.x;
		float height = _sr.sprite.bounds.size.y;

        _aspectRatio = width / height;
				
		float worldScreenHeight = Camera.main.orthographicSize * 2f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;


        if (X)
        {
            Vector3 xWidth = transform.localScale;
            xWidth.x = worldScreenWidth / width;
            transform.localScale = xWidth;
        }
        else
        {
            Vector3 xWidth = transform.localScale;
            xWidth.x = worldScreenHeight * _aspectRatio / width;
            transform.localScale = xWidth;
        }

        if (Y)
        {
            Vector3 yHeight = transform.localScale;
            yHeight.y = worldScreenHeight / height;
            transform.localScale = yHeight;
        }
        else
        {
            Vector3 yHeight = transform.localScale;
            yHeight.y = worldScreenWidth / _aspectRatio / height;
            transform.localScale = yHeight;
        }


		_height = Screen.height;
		_width = Screen.width;

        float h = height * transform.localScale.y;
        float delta = (worldScreenHeight - h) / 2f;

        switch (Alignment)
        {
            case VerticalAlignment.Up:
                transform.localPosition = Vector3.up * delta;
                break;
            case VerticalAlignment.Cemter:
                transform.localPosition = Vector3.zero;
                break;
            case VerticalAlignment.Down:
                transform.localPosition = Vector3.up * -delta;
                break;
            default:
                break;
        }

    }
}
