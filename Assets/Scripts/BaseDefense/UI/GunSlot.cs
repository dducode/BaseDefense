using UnityEngine;
using BaseDefense.AttackImplemention.Guns;
using UnityEngine.UI;

namespace BaseDefense.UI {

    [RequireComponent(typeof(Button))]
    public class GunSlot : MonoBehaviour {

        [SerializeField]
        private Gun gun;

        private Button m_thisButton;

        public int GunId => gun.Id;

        public RectTransform RectTransform { get; private set; }

        public void EnableButton () => m_thisButton.enabled = true;
        public void DisableButton () => m_thisButton.enabled = false;
        private void Awake () {
            m_thisButton = GetComponent<Button>();
            RectTransform = GetComponent<RectTransform>();
        }

    }

}