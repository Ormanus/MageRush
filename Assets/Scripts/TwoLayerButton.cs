using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Outloud.Common
{
    [ExecuteInEditMode]
    public class TwoLayerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public enum IconMode
        {
            None,
            Left,
            Right,
        }

        public UnityEvent OnPointerClick;
        public string text = "Button";
        public Sprite icon;
        public IconMode iconMode = IconMode.None;
        public float shadowDistance = 16f;

        Transform secondLayer;
        TMPro.TextMeshProUGUI textComponent;
        public bool pressed = false;

        private void Awake()
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this))
                return;
#endif
            textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            secondLayer = transform.GetChild(0);
            Init();
        }

        public void Init()
        {
            if (Application.isPlaying)
                secondLayer.localPosition += new Vector3(0f, shadowDistance, 0f);
            SetText(text);
            SetIconMode((int)iconMode);
            SetIcon(icon);
        }

        public void SetText(string s)
        {
#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabAsset(this))
            {
                Debug.LogWarning("Edited prefab text");
                return;
            }
#endif
            if (textComponent == null)
            {
                textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
                if (textComponent == null)
                {
                    Debug.LogWarning("Could not find text comopnent");
                    return;
                }
            }

            textComponent.text = s;
            textComponent.ForceMeshUpdate();
            textComponent.gameObject.SetActive(!string.IsNullOrEmpty(s));
            textComponent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textComponent.preferredWidth * textComponent.transform.localScale.x);
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(textComponent);
            EditorUtility.SetDirty(textComponent);
#endif
        }

        public void SetIcon(Sprite sprite)
        {
            var iconObj = GetIconObject();
            if (sprite == null && iconObj != null)
            {
                iconObj.SetActive(false);
                return;
            }
            if (iconObj != null && iconObj.TryGetComponent<Image>(out var img))
            {
                img.sprite = sprite;
#if UNITY_EDITOR
                PrefabUtility.RecordPrefabInstancePropertyModifications(img);
                EditorUtility.SetDirty(img);
#endif
            }
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(iconObj);
            EditorUtility.SetDirty(iconObj);
#endif
        }

        public void SetIconMode(int mode)
        {
            var obj = GetIconObject();
            if (obj == null)
                return;
            switch (mode)
            {
                case 0:
                    obj.SetActive(false);
                    break;
                case 1:
                    obj.SetActive(true);
                    obj.transform.SetAsFirstSibling();
                    break;
                case 2:
                    obj.SetActive(true);
                    obj.transform.SetAsLastSibling();
                    break;
                default:
                    break;
            }
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(secondLayer);
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            EditorUtility.SetDirty(secondLayer);
            EditorUtility.SetDirty(obj);
#endif
        }

        GameObject GetIconObject()
        {
            var iconTr = secondLayer.Find("Icon");
            if (iconTr == null)
            {
                var img = secondLayer.GetComponentInChildren<Image>();
                if (img && img.transform != secondLayer)
                    iconTr = img.transform;
            }
            if (iconTr)
                return iconTr.gameObject;
            return null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Down();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Up();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Press();
        }

        void Up()
        {
            if (pressed)
            {
                pressed = false;
                secondLayer.localPosition += new Vector3(0f, shadowDistance, 0f);
            }
        }

        void Down()
        {
            if (!pressed)
            {
                pressed = true;
                secondLayer.localPosition += new Vector3(0f, -shadowDistance, 0f);
            }
        }

        void Press()
        {
            if (pressed)
            {
                pressed = false;
                secondLayer.localPosition += new Vector3(0f, shadowDistance, 0f);
                OnPointerClick?.Invoke();
            }
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(TwoLayerButton))]
    public class TwoLayerButtonEditor : Editor
    {
        SerializedProperty OnPointerClick;
        SerializedProperty text;
        SerializedProperty icon;
        SerializedProperty iconMode;
        SerializedProperty shadowDistance;

        TwoLayerButton btn;

        public override void OnInspectorGUI()
        {
            btn = target as TwoLayerButton;

            SerializedObject serializedObject = new SerializedObject(target);

            OnPointerClick = serializedObject.FindProperty("OnPointerClick");
            text = serializedObject.FindProperty("text");
            icon = serializedObject.FindProperty("icon");
            iconMode = serializedObject.FindProperty("iconMode");
            shadowDistance = serializedObject.FindProperty("shadowDistance");

            if (PrefabUtility.IsPartOfPrefabAsset(btn))
            {
                DrawDefaultInspector();
                return;
            }

            bool objectsChanged = false;

            serializedObject.Update();
            EditorGUILayout.PropertyField(OnPointerClick);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Text:");

            string oldText = text.stringValue;
            text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.ExpandHeight(true), GUILayout.Height(32f));

            if (oldText != text.stringValue)
            {
                btn.SetText(text.stringValue);
                objectsChanged = true;
            }

            var oldMode = iconMode.enumValueIndex;
            EditorGUILayout.PropertyField(iconMode);
            if (oldMode != iconMode.enumValueIndex)
            {
                btn.SetIconMode(iconMode.enumValueIndex);
                objectsChanged = true;
            }

            if (iconMode.enumValueIndex != 0)
            {
                var oldIcon = icon.objectReferenceValue;
                EditorGUILayout.LabelField("Icon:");
                EditorGUILayout.PropertyField(icon, btn.icon != null ? new GUIContent(btn.icon.texture) : new GUIContent());
                if (oldIcon != icon.objectReferenceValue)
                {
                    btn.SetIcon(icon.objectReferenceValue as Sprite);
                    objectsChanged = true;
                }
            }

            EditorGUILayout.PropertyField(shadowDistance);

            if (objectsChanged)
                RecursiveSave(btn.transform);
            serializedObject.ApplyModifiedProperties();
        }

        void RecursiveSave(Transform tr)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(tr.gameObject);
            EditorUtility.SetDirty(tr.gameObject);

            for (int i = 0; i < tr.childCount; i++)
            {
                RecursiveSave(tr.GetChild(i));
            }
        }
    }
#endif
}