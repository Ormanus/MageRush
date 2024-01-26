using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageToggleSystem : MonoBehaviour
{
    public delegate bool GetValueDelegate();
    public GetValueDelegate Get;

    public delegate bool SetValueDelegate(bool value);
    public SetValueDelegate Set;


    public bool On = true;
    public Image TargetImage;
    public Sprite OnSprite;
    public Sprite OffSprite;

    public Text TargetText;

    bool _previousOn;

    void ChangeIcon(Sprite sprite)
    {
        TargetImage.sprite = sprite;
    }
    
    void Start()
    {
        if (Get != null)
            SetOn(Get.Invoke());
        Translator.OnLanguageChange.AddListener(UpdateText);
    }
    
    void Update()
    {
        if (_previousOn != On)
        {
            SetOn(On);
        }
    }

    public void UpdateText()
    {
        if (TargetText == null)
            return;

        switch (On)
        {
            case true:
                TargetText.text = Translator.GetWord("SETTINGS_TEXT_ON");
                break;
            case false:
                TargetText.text = Translator.GetWord("SETTINGS_TEXT_OFF");
                break;
        }
    }

    public void SetOn(bool on)
    {
        switch (on)
        {
            case true:
                ChangeIcon(OnSprite);
                break;
            case false:
                ChangeIcon(OffSprite);
                break;
        }
        On = on;
        UpdateText();
        _previousOn = on;

        if (Set != null)
            Set.Invoke(on);
    }

    private void OnDestroy()
    {
        // Prevent crashes by removing the destroyed object from the language change delegate
        Translator.OnLanguageChange.RemoveListener(UpdateText);
    }
}
