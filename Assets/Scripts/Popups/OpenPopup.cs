using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPopup : MonoBehaviour
{
    public GameObject PopupRoot;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => { PopupRoot.GetComponent<PopupController>().Enable(); });
    }
}
