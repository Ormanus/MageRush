using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Outloud.Common
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreaScaler : MonoBehaviour
    {
        public static UnityEvent onResolutionChange = new UnityEvent();

        private static bool screenChangeVarsInitialized = false;
        private static Vector2 lastResolution = Vector2.zero;
        private static Rect lastSafeArea = Rect.zero;

        private Canvas canvas;
        private RectTransform rectTransform;

        public RectTransform safeAreaTransform;

        void Awake()
        {
            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();

            if (safeAreaTransform == null)
                safeAreaTransform = transform.Find("SafeArea") as RectTransform;

            if (!screenChangeVarsInitialized)
            {
                lastResolution.x = Screen.width;
                lastResolution.y = Screen.height;
                lastSafeArea = Screen.safeArea;

                screenChangeVarsInitialized = true;
            }

            onResolutionChange.AddListener(ApplySafeArea);
        }

        void Start()
        {
            ApplySafeArea();
        }

        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Screen.safeArea != lastSafeArea)
                {
                    StartCoroutine(DelayedUpdate());
                }
            }
        }

        void ApplySafeArea()
        {
            if (safeAreaTransform == null)
                return;

            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
        }

        private static void SafeAreaChanged()
        {
            if (lastSafeArea == Screen.safeArea)
                return;
            lastSafeArea = Screen.safeArea;

            if (onResolutionChange != null)
            {
                onResolutionChange.Invoke();
            }
        }

        public Vector2 GetSafeAreaSize()
        {
            return safeAreaTransform.sizeDelta;
        }

        public static Rect GetSafeArea()
        {
            return lastSafeArea;
        }

        IEnumerator DelayedUpdate()
        {
            yield return null;
            SafeAreaChanged();
        }
    }
}