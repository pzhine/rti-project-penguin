using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using RedRunner.UI;

namespace RedRunner.UI
{
    public class IntroScreen : MonoBehaviour
    {
        [SerializeField]
        protected Button PlayButton;

        [SerializeField]
        protected InputField WorkerIdInput;

        private void Start()
        {
            PlayButton.SetButtonAction(() =>
            {
                GameManager.Singleton.StartGame(WorkerIdInput.text);
            });
        }

        //public void SetButtonAction(this Button button, Action action)
        //{
        //    button.onClick.RemoveAllListeners();
        //    button.onClick.AddListener(() => action());
        //}
    }

}