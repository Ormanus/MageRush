using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocalizedImage : MonoBehaviour
{
    public Sprite[] sprites;

    private void OnEnable()
    {
        Translator.OnLanguageChange.AddListener(onTranslate);
    }

    private void OnDisable()
    {
        Translator.OnLanguageChange.RemoveListener(onTranslate);
    }

    public void onTranslate()
    {
        GetComponent<Image>().sprite = sprites[(int)Translator.CurrentLanguage];
    }
}
