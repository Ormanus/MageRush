using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Translator
{
    public enum Language
    {
        Finnish,
        English,
        Swedish,
    }

    static string filePath = "Localization";
    public static Language CurrentLanguage;

    // Amount of languages + 1 for the key
    static int _languageOffset = 4;
    static string[] _words = null;

    public static UnityEvent OnLanguageChange = new UnityEvent();

    public static bool initialized = false;

    public static bool Init(bool force = false)
    {
        if (initialized && !force)
        {
            return true;
        }

        _languageOffset = System.Enum.GetValues(typeof(Language)).Length + 1;

        TextAsset file = Resources.Load<TextAsset>(filePath);
        if (!file)
        {
            Debug.LogWarning("Localization file not found!");
            initialized = false;
            return false;
        }
        string content = file.text;
        if (content == "")
        {
            content = System.Text.Encoding.Default.GetString(file.bytes);
        }

        {
            List<string> lines = new(content.Split(new string[] { System.Environment.NewLine, "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("//") || lines[i].StartsWith("#"))
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }
            _words = lines.ToArray();
        }

        for (int i = 0; i < _words.Length; i++)
        {
            _words[i] = _words[i].Replace("\\n", System.Environment.NewLine);
        }

        SwitchToSystemLanguage();
        initialized = true;

        return true;
    }

    public static string[] GetSortedKeys()
    {
        if (!Init())
            return null;

        string[] keys = new string[_words.Length / _languageOffset];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = _words[i * _languageOffset];
        }

        System.Array.Sort(keys);

        return keys;
    }

    public static void SwitchToSystemLanguage()
    {
        // Set default language
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                CurrentLanguage = Language.English;
            break;
            case SystemLanguage.Finnish:
                CurrentLanguage = Language.Finnish;
            break;
            case SystemLanguage.Swedish:
                CurrentLanguage = Language.Swedish;
            break;
            default:
                CurrentLanguage = Language.English;
            break;
        }

        Translate((int)CurrentLanguage);
    }

    public static Language GetLanguage()
    {
        return CurrentLanguage;
    }

    public static string GetWord(string ID)
    {
        if (!Init())
            return null;

        for (int i = 0; i < _words.Length; i += _languageOffset)
        {
            if (_words[i].Equals(ID, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return _words[i + (int)CurrentLanguage + 1];
            }
        }
        Debug.LogWarning("Word with ID " + ID  + " not found!");
        return ID;
    }

    public static void Translate(int newLanguage)
    {
        if(newLanguage != (int)CurrentLanguage)
        {
            CurrentLanguage = (Language)newLanguage;
            OnLanguageChange.Invoke();
        }
    }
}
