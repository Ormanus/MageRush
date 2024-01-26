using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonReact : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position += new Vector3(-1, -1, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.position += new Vector3(1, 1, 0);
    }
}
