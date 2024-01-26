using UnityEngine;
using UnityEngine.UI;

namespace Outloud.Common
{
    public class ClickSound : MonoBehaviour
    {
        public AudioClip clickSound;

        private void Start()
        {
            var button = GetComponent<Button>();
            if (button)
                button.onClick.AddListener(PlaySound);
            var TLButton = GetComponent<TwoLayerButton>();
            if (TLButton)
                TLButton.OnPointerClick.AddListener(PlaySound);
        }

        public void PlaySound()
        {
            if (AudioManager.Exists)
                AudioManager.PlaySound(clickSound);
        }
    }
}