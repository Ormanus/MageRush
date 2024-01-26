using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalizedText : MonoBehaviour
{
    public string LocalizationID;

    private void OnEnable()
    {
        OnTranslate();
        Translator.OnLanguageChange.AddListener(OnTranslate);
    }

    private void OnDisable()
    {
        Translator.OnLanguageChange.RemoveListener(OnTranslate);
    }

    private void OnTranslate()
    {
        if (TryGetComponent<Text>(out var textComponent))
            textComponent.text = Translator.GetWord(LocalizationID);
        else if (TryGetComponent<TextMesh>(out var textMeshComponent))
            textMeshComponent.text = Translator.GetWord(LocalizationID);
        else if (TryGetComponent<TMPro.TextMeshPro>(out var textMeshProComponent))
            textMeshProComponent.text = Translator.GetWord(LocalizationID);
        else if (TryGetComponent<TMPro.TextMeshProUGUI>(out var textMeshUGUIComponent))
            textMeshUGUIComponent.text = Translator.GetWord(LocalizationID);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    SerializedProperty LocalizationID;

    string[] keys;
    string query = "";

    private void OnEnable()
    {
        Translator.Init();
        LocalizationID = serializedObject.FindProperty("LocalizationID");
        InitKeys();
    }

    void InitKeys()
    {
        keys = Translator.GetSortedKeys();
    }

    public override void OnInspectorGUI()
    {
        GUIStyle red = new GUIStyle(GUIStyle.none);
        red.normal.textColor = Color.red;

        serializedObject.Update();

        if (GUILayout.Button("Re-init translator"))
        {
            Translator.Init(true);
            InitKeys();
        }

        if (Translator.initialized)
        {
            query = EditorGUILayout.TextField("Search box: ", query);

            List<string> searchedKeys = new List<string>(keys);
            for (int i = 0; i < searchedKeys.Count; i++)
            {
                if (!searchedKeys[i].ToUpper().Contains(query.ToUpper()))
                {
                    searchedKeys.RemoveAt(i);
                    i--;
                }
            }

            if (searchedKeys.Count == 0)
            {
                GUILayout.Label("No results with this query!", red);
                return;
            }

            int selection = -1;
            for (int i = 0; i < searchedKeys.Count; i++)
            {
                if (searchedKeys[i].Equals(LocalizationID.stringValue, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    selection = i;
                    break;
                }
            }

            selection = EditorGUILayout.Popup(selection == -1 ? 0 : selection, searchedKeys.ToArray());

            if (selection != -1)
                LocalizationID.stringValue = searchedKeys[selection];
        }
        else
        {
            GUILayout.Label("Translator not initialized!", red);
            EditorGUILayout.PropertyField(LocalizationID);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif