using System;
using System.IO;
using BaseDefense.Characters;
using BaseDefense.SaveSystem;
using UnityEngine;
using Zenject;

namespace BaseDefense.UI {

    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class GunSelectWindow : MonoBehaviour {

        ///<summary>Рамка для выбранного игроком оружия</summary>
        [SerializeField, Tooltip("Рамка для выбранного игроком оружия")]
        private RectTransform frame;

        [SerializeField]
        private RectTransform content;

        [Inject]
        private PlayerCharacter m_player;
        
        public Canvas Canvas { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }

        private const string FILE_NAME = "uiSave.dat";


        public void SelectGun (GunSlot gunSlot) {
            m_player.SelectGun(gunSlot.gun.Id);
        }


        public void OnSelectGun (RectTransform rectTransform) {
            frame.anchoredPosition = rectTransform.anchoredPosition;
        }


        private void Awake () {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
            Load();
            Application.quitting += Save;
        }


        private void OnApplicationPause (bool pauseStatus) {
            if (pauseStatus)
                Save();
        }


        private void Start () {
            Canvas.enabled = false;
            CanvasGroup.alpha = 0;
        }


        private void Save () {
            var path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new UnityWriter(binaryWriter);
            writer.Write(frame.anchoredPosition);
            writer.Write(content.anchoredPosition);
        }


        private void Load () {
            var path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            if (!File.Exists(path))
                return;

            var binaryData = File.ReadAllBytes(path);
            using var binaryReader = new BinaryReader(new MemoryStream(binaryData));
            var reader = new UnityReader(binaryReader);
            frame.anchoredPosition = reader.ReadPosition();
            content.anchoredPosition = reader.ReadPosition();
        }

    }

}