using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using BaseDefense.Items;
using BaseDefense.SaveSystem;
using BaseDefense.UI;

namespace BaseDefense
{
    public class Inventory
    {
        private string m_inventoryData;
        private readonly DisplayingUI m_ui;
        private const string INVENTORY_DATA_FILE_NAME = "inventoryData.dat";
        private const string PASSWORD = "4u7b8O-0j2lvGHtTZrQ.cV3aN?ydosUwWE9z,ShYkACm6InBgJRi!K_DMep1XLxq";

        public Inventory(DisplayingUI ui)
        {
            m_ui = ui;
            var data = LoadInventory() ?? new InventoryData
            {
                moneys = PlayerPrefs.GetInt("Money", 0),
                gems = PlayerPrefs.GetInt("Gem", 0)
            };
            m_inventoryData = EncodeData(data);
            ui.UpdateUI(data.moneys, data.gems);
            Application.wantsToQuit += () =>
            {
                SaveInventory();
                return true;
            };
        }

        ///<summary>Кладёт предмет в инвентарь и сохраняет значение в PlayerPrefs</summary>
        public void PutItem(Item item)
        {
            var data = DecodeData(m_inventoryData);
            switch (item)
            {
                case Money:
                    data.moneys += 5;
                    PlayerPrefs.SetInt("Money", data.moneys);
                    break;
                case Gem:
                    data.gems++;
                    PlayerPrefs.SetInt("Gem", data.gems);
                    break;
                default:
                    throw new NotImplementedException($"Предмет {item} не реализован");
            }

            m_inventoryData = EncodeData(data);
            item.DestroyItem();
            PlayerPrefs.Save();
            m_ui.UpdateUI(data.moneys, data.gems);
        }

        private void SaveInventory()
        {
            var path = Path.Combine(Application.persistentDataPath, INVENTORY_DATA_FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            var data = JsonUtility.ToJson(DecodeData(m_inventoryData));
            var encryptData = AES.Encrypt(data, PASSWORD);
            writer.Write(encryptData);
        }

        private InventoryData? LoadInventory()
        {
            var path = Path.Combine(Application.persistentDataPath, INVENTORY_DATA_FILE_NAME);
            var reader = GameDataStorage.GetDataReader(path);
            if (reader is null)
                return null;

            var data = AES.Decrypt(reader.ReadString(), PASSWORD);
            return JsonUtility.FromJson<InventoryData>(data);
        }

        private static string EncodeData(InventoryData data)
        {
            var jsonData = JsonUtility.ToJson(data);
            return B64X.Encode(jsonData);
        }

        private static InventoryData DecodeData(string jsonData)
        {
            var data = B64X.Decode(jsonData);
            return JsonUtility.FromJson<InventoryData>(data);
        }

        [Serializable]
        public struct InventoryData
        {
            public int moneys;
            public int gems;

            public byte[] GetBytes()
            {
                var moneysBytes = BitConverter.GetBytes(moneys);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(moneysBytes);

                var gemsBytes = BitConverter.GetBytes(gems);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(gemsBytes);

                return moneysBytes.Concat(gemsBytes).ToArray();
            }
        }
    }
}


