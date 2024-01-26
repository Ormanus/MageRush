using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Outloud.Common
{
    public class ButtonClose : MonoBehaviour
    {
        public UnityEvent OnClose;

        // Highest layer is closed first
        public int Layer = 3;

        private void OnEnable()
        {
            Open();
        }

        private void OnDisable()
        {
            BackKeyHandler.Remove(this);
        }

        public virtual void Close()
        {
            Button button = GetComponent<Button>();
            if (button != null)
                button.onClick.Invoke();
        }

        public virtual int GetPriority()
        {
            PopupController popupController = GetComponentInParent<PopupController>();
            if (popupController != null)
            {
                if (popupController.GetAnimationState() == PopupController.AnimationState.Out)
                {
                    return -1;
                }
            }
            return Layer;
        }

        public virtual void Open()
        {
            BackKeyHandler.Add(this);
        }
    }
}