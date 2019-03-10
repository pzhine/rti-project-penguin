using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RedRunner.UI
{
    public class UIDialog : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform m_RectTransform;

        [SerializeField]
        protected Button m_OkButton;

        private void Start()
        {
            m_OkButton.SetButtonAction(HideDialog);
        }

        public void ShowDialog()
        {
            m_RectTransform.localScale = new Vector3(1, 1, 1);
        }

        public void HideDialog()
        {
            m_RectTransform.localScale = new Vector3(0, 0, 0);
        }
    }
}
