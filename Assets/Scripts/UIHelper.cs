using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Outloud.Common.UIHelper;

namespace Outloud.Common
{
    public static class UIHelper
    {
        #region Modifiers
        public abstract class AnimationModifier
        {

        }

        public class DelayModifier : AnimationModifier
        {
            public float _time;
            public DelayModifier(float time) 
            {
                _time = time;
            }
        }

        public class DurationModifier : AnimationModifier
        {
            public float _time;
            public DurationModifier(float time)
            {
                _time = time;
            }
        }

        public class EndActionModifier : AnimationModifier
        {
            public Action _action;
            public EndActionModifier(Action endEvent)
            {
                _action = endEvent;
            }
        }

        public static AnimationModifier Delay(float duration)
        {
            return new DelayModifier(duration);
        }

        public static AnimationModifier Duration(float duration)
        {
            return new DurationModifier(duration);
        }

        public static AnimationModifier OnEnd(Action action)
        {
            return new EndActionModifier(action);
        }

        static IEnumerator UseDelay(AnimationModifier[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is DelayModifier)
                {
                    yield return new WaitForSeconds((parameters[i] as DelayModifier)._time);
                }
            }
        }

        static void UseEndAction(AnimationModifier[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is EndActionModifier)
                {
                    (parameters[i] as EndActionModifier)._action?.Invoke();
                }
            }
        }

        static float GetDuration(AnimationModifier[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is DurationModifier)
                {
                    return (parameters[i] as DurationModifier)._time;
                }
            }
            return 1f;
        }
        #endregion

        #region UI Utility Functions

        public static IEnumerator MoveFromTo(GameObject movingObject, Vector3 pointA, Vector3 pointB, params AnimationModifier[] modifiers)
        {
            UseDelay(modifiers);
            float duration = GetDuration(modifiers);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                movingObject.transform.position = Vector3.Lerp(pointA, pointB, t);
                yield return 0;
            }
            movingObject.transform.position = pointB;
            UseEndAction(modifiers);
        }

        public static IEnumerator FlashObject(Graphic flashObject, Color flashColor, int repeats = 1, params AnimationModifier[] modifiers)
        {
            UseDelay(modifiers);
            float duration = GetDuration(modifiers);
            Color startColor = flashObject.color;
            for (int i = 0; i < repeats; i++)
            {
                flashObject.color = flashColor;
                yield return new WaitForSeconds(duration);
                flashObject.color = startColor;
                if (i <= repeats)
                    yield return new WaitForSeconds(duration);
            }
            UseEndAction(modifiers);
        }

        public static IEnumerator RotateObject(GameObject rotateObject, Vector3 startAngle, Vector3 endAngle, params AnimationModifier[] modifiers)
        {
            Quaternion startRotation = Quaternion.Euler(startAngle);
            Quaternion endRotation = Quaternion.Euler(endAngle);

            float t = 0.0f;

            UseDelay(modifiers);
            float duration = GetDuration(modifiers);

            // animate
            while (t < 1.0f)
            {
                t += Time.deltaTime / duration;
                rotateObject.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);// % 180f;
                //Debug.Log(xRotation);
                //rotateObject.transform.rotation.x = new Vector3(xRotation, rotateObject.transform.eulerAngles.y, rotateObject.transform.eulerAngles.z);
                yield return null;
            }
            UseEndAction(modifiers);
        }

        public static IEnumerator FadeAlpha(Graphic graphic, float targetAlpha, params AnimationModifier[] modifiers)
        {
            Color c = graphic.color;
            float startAlpha = c.a;
            float t = 0f;

            UseDelay(modifiers);
            float duration = GetDuration(modifiers);

            if (duration <= 0f)
            {
                graphic.color = new Color(c.r, c.g, c.b, targetAlpha);
                UseEndAction(modifiers);
                yield break;
            }

            while (t < 1f)
            {
                graphic.color = new Color(c.r, c.g, c.b, Mathf.Lerp(startAlpha, targetAlpha, t));
                t += Time.deltaTime / duration;
                yield return null;
            }
            graphic.color = new Color(c.r, c.g, c.b, targetAlpha);
            UseEndAction(modifiers);
        }

        public static IEnumerator FadeColor(Graphic graphic, Color targetColor, params AnimationModifier[] modifiers)
        {
            UseDelay(modifiers);
            float duration = GetDuration(modifiers);
            Color c = graphic.color;
            float t = 0f;
            while (t < 1f)
            {
                graphic.color = Color.Lerp(c, targetColor, t);
                t += Time.deltaTime / duration;
                yield return null;
            }
            graphic.color = targetColor;
            UseEndAction(modifiers);
        }

        #endregion

        #region Find Tagged Objects

        public static GameObject FindComponentInChildWithTag(GameObject parent, string tag) 
        {
            Transform t = parent.transform; 
            foreach (Transform tr in t)
            {
                Debug.Log(tr.tag);
                if (tr.CompareTag(tag))
                {
                    return tr.gameObject;
                }
            }
            return null;
        }

        public static List<GameObject> FindComponentsInChildWithTag(GameObject parent, string tag)
        {
            List<GameObject> r = new List<GameObject>();
            Transform t = parent.transform;
            foreach (Transform tr in t)
            {
                if (tr.CompareTag(tag))
                {
                    r.Add(tr.gameObject);
                }
            }
            return r;
        }

        public static GameObject FindComponentInParentWithTag(GameObject go, string tag)
        {
            Transform t = go.transform;
            while(t)
            {
                if (t.CompareTag(tag))
                {
                    return t.gameObject;
                }
                t = t.parent;
            }
            return null;
        }
        #endregion
    }
}