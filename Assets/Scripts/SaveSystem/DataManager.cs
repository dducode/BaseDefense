using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BaseDefense.SaveSystem {

    public static class DataManager {

        public static void SaveObjects (string fileName, params IPersistentObject[] objects) {
            if (objects.Length is 0) {
                Debug.LogWarning("");
                return;
            }

            var localPath = Path.Combine(Application.persistentDataPath, $"{fileName}.bytes");
            using var binaryWriter = new BinaryWriter(File.Open(localPath, FileMode.OpenOrCreate));
            var unityWriter = new UnityWriter(binaryWriter);

            foreach (var obj in objects)
                obj.Save(unityWriter);
        }


        public static bool LoadObjects (string fileName, params IPersistentObject[] objects) {
            var localPath = Path.Combine(Application.persistentDataPath, $"{fileName}.bytes");
            if (!File.Exists(localPath))
                return false;

            var data = File.ReadAllBytes(localPath);
            var unityReader = new UnityReader(new BinaryReader(new MemoryStream(data)));

            foreach (var obj in objects)
                obj.Load(unityReader);

            return true;
        }


        [MenuItem("Data Manager/Remove Data")]
        private static void RemoveData () {
            var data = Directory.GetFiles(Application.persistentDataPath);

            foreach (var filePath in data)
                File.Delete(filePath);

            Debug.Log("Successfully deleted");
        }


        [MenuItem("Data Manager/Remove Data", true)]
        private static bool ValidateRemoveData () {
            return Directory.GetFiles(Application.persistentDataPath).Length > 0;
        }

    }

}