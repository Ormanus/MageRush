using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outloud.Common
{
    public class SettingsData
    {
        public string playerName;
        public bool soundsOn;

        // Default Settings
        public SettingsData()
        {
            playerName = "NO_NAME";
            soundsOn = true;
        }

        // Copy
        public SettingsData(SettingsData toCopy)
        {
            playerName = toCopy.playerName;
            soundsOn = toCopy.soundsOn;
        }
    }

    public class Settings : MonoBehaviour
    {
        public static SettingsData CurrentSettings = new SettingsData();

        private void Start()
        {
            Load();
        }

        public void Save()
        {
            string data = JsonUtility.ToJson(CurrentSettings);
            PlayerPrefs.SetString("Settings", data);
        }

        public void Load()
        {
            string data = PlayerPrefs.GetString("Settings", "");
            if (data == "")
            {
                CurrentSettings = new SettingsData();
                return;
            }
            var loadedSettings = JsonUtility.FromJson<SettingsData>(data);
        }
    }
}