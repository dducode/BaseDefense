using BaseDefense.Characters;
using SaveSystem;
using UnityEngine;
using Zenject;

namespace BaseDefense.UI {

    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class GunSelectWindow : MonoBehaviour, IPersistentObject {

        ///<summary>Рамка для выбранного игроком оружия</summary>
        [SerializeField, Tooltip("Рамка для выбранного игроком оружия")]
        private RectTransform frame;

        [SerializeField]
        private RectTransform content;

        [Inject]
        private PlayerCharacter m_player;

        public Canvas Canvas { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }


        public void SelectGun (GunSlot gunSlot) {
            m_player.SelectGun(gunSlot.gun.Id);
        }


        public void OnSelectGun (RectTransform rectTransform) {
            frame.anchoredPosition = rectTransform.anchoredPosition;
        }


        public void Save (UnityWriter writer) {
            writer.Write(frame.anchoredPosition);
            writer.Write(content.anchoredPosition);
        }


        public void Load (UnityReader reader) {
            frame.anchoredPosition = reader.ReadPosition();
            content.anchoredPosition = reader.ReadPosition();
        }


        private const string FILE_NAME = "uiSave";


        private void Awake () {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
            DataManager.LoadObjects(FILE_NAME, this);
            Application.quitting += () => DataManager.SaveObjects(FILE_NAME, this);
        }


        private void Start () {
            Canvas.enabled = false;
            CanvasGroup.alpha = 0;
        }


        private void OnApplicationPause (bool pauseStatus) {
            if (pauseStatus)
                DataManager.SaveObjects(FILE_NAME, this);
        }

    }

}