using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Outloud.Common
{
    public class RadioButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        static readonly List<RadioButton> radioButtons = new();

        public bool StartSelected = false;

        public Image toplayer;
        public Image bottomlayer;

        [Space]
        public Color SelectedMainColor;
        public Color SelectedShadowColor;
        public Color UnselectedMainColor;
        public Color UnselectedShadowColor;

        public bool Selected { get; private set; }
        public int Group = 0;
        public float offset = 8f;

        private void Awake()
        {
            toplayer.transform.position += offset * Vector3.up;

            radioButtons.Add(this);
            if (StartSelected)
                Select();
        }

        private void OnDestroy()
        {
            radioButtons.Remove(this);
        }

        void Deselect()
        {
            if (!Selected)
                return;
            Selected = false;

            toplayer.transform.position += offset * 2 * Vector3.up;
            toplayer.color = UnselectedMainColor;
            bottomlayer.color = UnselectedShadowColor;
        }

        public void Select()
        {
            if (Selected)
                return;
            Selected = true;
            foreach (var radioButton in radioButtons)
            {
                if (radioButton.Group == Group && radioButton != this)
                {
                    radioButton.Deselect();
                }
            }

            toplayer.transform.position += offset * 2 * Vector3.down;
            toplayer.color = SelectedMainColor;
            bottomlayer.color = SelectedShadowColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Select();
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
    }

}