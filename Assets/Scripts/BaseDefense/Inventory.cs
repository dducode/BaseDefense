using System;
using System.IO;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using UnityEngine;
using BaseDefense.Items;
using BaseDefense.SaveSystem;

namespace BaseDefense {

    public class Inventory : MonoBehaviour {

        private string m_inventoryData;
        private const string INVENTORY_DATA_FILE_NAME = "inventoryData.dat";
        private const string PASSWORD = "4u7b8O-0j2lvGHtTZrQ.cV3aN?ydosUwWE9z,ShYkACm6InBgJRi!K_DMep1XLxq";


        private void Awake () {
            var data = LoadInventory() ?? new InventoryData {
                moneys = PlayerPrefs.GetInt("Money", 0),
                gems = PlayerPrefs.GetInt("Gem", 0)
            };
            m_inventoryData = EncodeData(data);
            Application.wantsToQuit += () => {
                SaveInventory();
                return true;
            };
        }


        private void Start () {
            var data = DecodeData(m_inventoryData);
            Messenger.SendMessage(new UpdateMoneysMessage(data.moneys));
            Messenger.SendMessage(new UpdateGemsMessage(data.gems));
        }


        ///<summary>Кладёт предмет в инвентарь</summary>
        public void PutItem (Item item) {
            var data = DecodeData(m_inventoryData);

            switch (item) {
                case Money:
                    data.moneys += 5;
                    Messenger.SendMessage(new UpdateMoneysMessage(data.moneys));
                    break;
                case Gem:
                    data.gems++;
                    Messenger.SendMessage(new UpdateGemsMessage(data.gems));
                    break;
                default:
                    throw new NotImplementedException($"Предмет {item} не реализован");
            }

            m_inventoryData = EncodeData(data);
            item.DestroyItem();
            PlayerPrefs.Save();
        }


        private void SaveInventory () {
            var path = Path.Combine(Application.persistentDataPath, INVENTORY_DATA_FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            var data = JsonUtility.ToJson(DecodeData(m_inventoryData));
            var encryptData = Aes.Encrypt(data, PASSWORD);
            writer.Write(encryptData);
        }


        private InventoryData? LoadInventory () {
            var path = Path.Combine(Application.persistentDataPath, INVENTORY_DATA_FILE_NAME);
            var reader = GameDataStorage.GetDataReader(path);

            if (reader is null)
                return null;

            var data = Aes.Decrypt(reader.ReadString(), PASSWORD);

            return JsonUtility.FromJson<InventoryData>(data);
        }


        private static string EncodeData (InventoryData data) {
            var jsonData = JsonUtility.ToJson(data);

            return B64X.Encode(jsonData);
        }


        private static InventoryData DecodeData (string jsonData) {
            var data = B64X.Decode(jsonData);

            return JsonUtility.FromJson<InventoryData>(data);
        }



        [Serializable]
        public struct InventoryData {

            public int moneys;
            public int gems;

        }

    }

}