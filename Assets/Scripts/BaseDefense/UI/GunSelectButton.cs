using UnityEngine;
using UnityEngine.UI;

namespace BaseDefense.UI {

    [RequireComponent(typeof(Button))]
    public class GunSelectButton : MonoBehaviour {

        private Button m_thisButton;


        public void EnableButton () {
            m_thisButton.enabled = true;
        }


        public void DisableButton () {
            m_thisButton.enabled = false;
        }


        private void Awake () {
            m_thisButton = GetComponent<Button>();
            GetComponent<RectTransform>();
        }

    }

}