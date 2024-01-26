using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Outloud.Common
{
    public class GoToScene : MonoBehaviour
    {
        public string scene;
        public Image blackScreen;
        void EnterScene()
        {
            SceneManager.LoadScene(scene);
        }

        public void OnClick()
        {
            if (blackScreen != null)
            {
                UIHelper.FadeAlpha(blackScreen, 1f, UIHelper.OnEnd(EnterScene));
            }
            else
            {
                EnterScene();
            }
        }
    }
}
