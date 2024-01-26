using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Outloud.Common
{
    public class BackKeyHandler : MonoBehaviour
    {
        private static List<ButtonClose> ActiveButtons = new List<ButtonClose>();

        public static void Add(ButtonClose button)
        {
            ActiveButtons.Add(button);
        }

        public static void Remove(ButtonClose button)
        {
            ActiveButtons.Remove(button);
        }

        public static void ReturnPressed()
        {
            if (ActiveButtons == null)
                return;

            int maxLayer = -1;
            ButtonClose toClose = null;
            foreach (var button in ActiveButtons)
            {
                if (button.GetPriority() > maxLayer)
                {
                    toClose = button;
                    maxLayer = button.GetPriority();
                }
            }
            if (toClose != null)
            {
                toClose.Close();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
                ReturnPressed();
            }
        }

        // For inspector
        public string GetButtonNames()
        {
            string str = "";
            for (int i = 0; i < ActiveButtons.Count; i++)
            {
                string line = "";
                var tr = ActiveButtons[i].transform;
                while (tr != null)
                {
                    line = tr.name + '/' + line;
                    tr = tr.parent;
                }
                str += line;
                if (i < ActiveButtons.Count - 1)
                    str += '\n';
            }
            return str;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(BackKeyHandler))]
    public class BackKeyHandlerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var handler = target as BackKeyHandler;

            GUILayout.Label(handler.GetButtonNames());
        }
    }

#endif
}