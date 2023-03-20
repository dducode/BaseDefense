using System;
using System.IO;
using BaseDefense.Characters;
using BaseDefense.SaveSystem;
using UnityEngine;
using Zenject;

namespace BaseDefense.UI {

    public class ShopWindow : MonoBehaviour {

        ///<summary>Рамка для выбранного игроком оружия</summary>
        [SerializeField, Tooltip("Рамка для выбранного игроком оружия")]
        private RectTransform frame;

        [SerializeField]
        private RectTransform content;

        [Inject]
        private PlayerCharacter m_player;

        private const string FILE_NAME = "uiSave.dat";


        public void SelectGun (GunSlot slot) {
            frame.anchoredPosition = slot.RectTransform.anchoredPosition;
            m_player.SelectGun(slot.GunId);
        }


        private void Awake () {
            Load();
            Application.wantsToQuit += () => {
                Save();
                return true;
            };
        }


        private void Save () {
            var path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            writer.Write(frame.anchoredPosition);
            writer.Write(content.anchoredPosition);
        }


        private void Load () {
            var path = Path.Combine(Application.persistentDataPath, FILE_NAME);
            var reader = GameDataStorage.GetDataReader(path);

            if (reader is null)
                return;

            frame.anchoredPosition = reader.ReadPosition();
            content.anchoredPosition = reader.ReadPosition();
        }

    }

}