using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Outloud.Common
{
    public class ClosePopup : ButtonClose, IPointerClickHandler
    {
        public GameObject PopupRoot;

        public void OnPointerClick(PointerEventData eventData)
        {
            PopupRoot.GetComponent<PopupController>().Disable();
        }
        public override void Close()
        {
            OnPointerClick(null);
            base.Close();
        }
    }
}
