using System;
using System.Collections.Generic;
using UnityEngine;
using BaseDefense.Items;
using BaseDefense.Messages;
using BaseDefense.Messages.UpdateCurrencyMessages;
using BroadcastMessages;
using SaveSystem;
using SaveSystem.UnityHandlers;

namespace BaseDefense {

    public class Inventory : MonoBehaviour, IPersistentObject {

        private string m_inventoryData;
        private const string InventoryDataFileName = "inventoryData.bytes";


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
        }


        public void PurchaseGun (int gunId, int price) {
            var data = DecodeData(m_inventoryData);
            data.gunIds.Add(gunId);
            data.moneys -= price;
            Messenger.SendMessage(new UpdateMoneysMessage(data.moneys));
            Messenger.SendMessage(new UnlockedGunsMessage(data.gunIds));
            m_inventoryData = EncodeData(data);
            DataManager.SaveObject(InventoryDataFileName, this);
        }


        public void PurchaseUpgrade (int price) {
            var data = DecodeData(m_inventoryData);
            data.gems -= price;
            Messenger.SendMessage(new UpdateGemsMessage(data.gems));
            m_inventoryData = EncodeData(data);
            DataManager.SaveObject(InventoryDataFileName, this);
        }


        private const string Password = "4u7b8O-0j2lvGHtTZrQ.cV3aN?ydosUwWE9z,ShYkACm6InBgJRi!K_DMep1XLxq";


        public void Save () {
            DataManager.SaveObject(InventoryDataFileName, this);
        }


        public void Save (UnityWriter writer) {
            var data = JsonUtility.ToJson(DecodeData(m_inventoryData));
            var encryptData = Aes.Encrypt(data, Password);
            writer.Write(encryptData);
        }


        public void Load (UnityReader reader) {
            var data = Aes.Decrypt(reader.ReadString(), Password);
            var inventoryData = JsonUtility.FromJson<InventoryData>(data);
            m_inventoryData = EncodeData(inventoryData);
        }


        private void Awake () {
            if (!DataManager.LoadObject(InventoryDataFileName, this)) {
                m_inventoryData = EncodeData(new InventoryData {
                    moneys = PlayerPrefs.GetInt("Money", 0),
                    gems = PlayerPrefs.GetInt("Gem", 0)
                });
            }

            Application.quitting += () => { DataManager.SaveObject(InventoryDataFileName, this); };
        }


        private void OnApplicationPause (bool pauseStatus) {
            if (pauseStatus)
                DataManager.SaveObject(InventoryDataFileName, this);
        }


        private void Start () {
            var data = DecodeData(m_inventoryData);
            Messenger.SendMessage(new UpdateMoneysMessage(data.moneys));
            Messenger.SendMessage(new UpdateGemsMessage(data.gems));
            Messenger.SendMessage(new UnlockedGunsMessage(data.gunIds));
        }


#if UNITY_EDITOR
        private void Update () {
            if (Input.GetKeyDown(KeyCode.M)) {
                var data = DecodeData(m_inventoryData);
                data.moneys += 1000;
                Messenger.SendMessage(new UpdateMoneysMessage(data.moneys));
                m_inventoryData = EncodeData(data);
            }
            else if (Input.GetKeyDown(KeyCode.G)) {
                var data = DecodeData(m_inventoryData);
                data.gems += 250;
                Messenger.SendMessage(new UpdateGemsMessage(data.gems));
                m_inventoryData = EncodeData(data);
            }
        }
#endif


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
            public List<int> gunIds;

        }

    }

}