using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Outloud.Common
{
    public class OpenExitPopup : ButtonClose, IPointerClickHandler
    {
        public GameObject PopupRoot;

        public void OnPointerClick(PointerEventData eventData)
        {
            PopupRoot.GetComponent<PopupController>().Enable();
        }
        public override void Close()
        {
            OnPointerClick(null);
            //base.Close();
        }
    }
}