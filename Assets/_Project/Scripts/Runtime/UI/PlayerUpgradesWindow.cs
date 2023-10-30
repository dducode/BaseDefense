using UnityEngine;

namespace BaseDefense.UI {

    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class PlayerUpgradesWindow : MonoBehaviour {

        public Canvas Canvas { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }


        private void Awake () {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
        }


        private void Start () {
            Canvas.enabled = false;
            CanvasGroup.alpha = 0;
        }

    }

}