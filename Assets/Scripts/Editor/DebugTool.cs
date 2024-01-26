using UnityEngine;
using UnityEditor;

namespace Outloud.Common
{
    public class DebugTool : EditorWindow
    {
        [MenuItem("Window/Debug Tools")]
        static void Init()
        {
            DebugTool window = (DebugTool)EditorWindow.GetWindow(typeof(DebugTool));
            window.Show();
        }

        void OnGUI()
        {
            var redText = new GUIStyle(GUI.skin.button);
            redText.normal.textColor = Color.red;

            if (GUILayout.Button("Reset Save", redText))
            {
                PlayerPrefs.DeleteAll();
            }

            GUILayout.Space(100f);

            if (GUILayout.Button(" ◀︎ Native Back Button"))
            {
                BackKeyHandler.ReturnPressed();
            }
        }
    }
}