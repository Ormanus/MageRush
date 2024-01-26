using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outloud.Common
{
    public class AudioManager : MonoBehaviour
    {
        [System.Serializable]
        public struct AudioSelection
        {
            public string audioID;
            public AudioClip[] clips;
        }

        public AudioSelection[] AudioList;
        public AudioSource Source;

        List<GameObject> loops = new List<GameObject>();

        static AudioManager instance;

        private void Awake()
        {
            if (instance != null && instance)
                Destroy(instance);

            instance = this;
        }

        public static bool Exists
        {
            get => instance != null && instance;
        }

        public static void PlaySound(string ID)
        {
            var clip = GetClip(ID);
            if (clip != null)
                instance.Source.PlayOneShot(clip);
        }

        public static void PlaySound(AudioClip clip)
        {
            if (clip != null)
                instance.Source.PlayOneShot(clip);
        }

        static AudioClip GetClip(string ID)
        {
            foreach (var item in instance.AudioList)
            {
                if (item.audioID.Equals(ID, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    var clip = item.clips[Random.Range(0, item.clips.Length)];
                    return clip;
                }
            }
            Debug.LogWarning($"Sound '{ID}' not found!");
            return null;
        }

        public static void PlayLoop(string ID)
        {
            var clip = GetClip(ID);
            if (clip != null)
            {
                var go = new GameObject(ID);
                var source = go.AddComponent<AudioSource>();
                source.loop = true;
                source.clip = clip;
                source.Play();
                instance.loops.Add(go);
            }
        }

        public static void StopLoop(string ID)
        {
            foreach (var loop in instance.loops)
            {
                if (loop.name == ID)
                {
                    instance.loops.Remove(loop);
                    Destroy(loop);
                    return;
                }
            }
        }

        public static void StopAllLoops()
        {
            foreach (var loop in instance.loops)
            {
                Destroy(loop);
            }
            instance.loops.Clear();
        }
    }
}
