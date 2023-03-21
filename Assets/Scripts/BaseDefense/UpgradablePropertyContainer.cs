using System.IO;
using BaseDefense.SaveSystem;
using UnityEngine;

namespace BaseDefense {

    public static class UpgradablePropertyContainer {

        private static UpgradableProperty SpeedProperty { get; set; }
        private static UpgradableProperty MaxHealthProperty { get; set; }
        private static UpgradableProperty MaxCapacityProperty { get; set; }


        [RuntimeInitializeOnLoadMethod]
        private static void Initialize () {
            SpeedProperty = Resources.Load<UpgradableProperty>("Max Speed Property/Speed Upgradable Property");
            MaxHealthProperty = Resources.Load<UpgradableProperty>("Max Health Property/Health Upgradable Property");
            MaxCapacityProperty = Resources.Load<UpgradableProperty>("Max Capacity Property/Capacity Upgradable Property");

            if (!Load()) {
                SpeedProperty.CurrentStep = SpeedProperty.upgradablePropertySteps[0];
                MaxHealthProperty.CurrentStep = MaxHealthProperty.upgradablePropertySteps[0];
                MaxCapacityProperty.CurrentStep = MaxCapacityProperty.upgradablePropertySteps[0];
            }

            Application.wantsToQuit += () => {
                Save();
                return true;
            };
        }
        
        
        private const string FILE_NAME = "PropertySave.dat";
        private static readonly string Path = System.IO.Path.Combine(Application.persistentDataPath, FILE_NAME);

        private static void Save () {
            using var binaryWriter = new BinaryWriter(File.Open(Path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            writer.Write(SpeedProperty.CurrentStep.stepCount);
            writer.Write(MaxHealthProperty.CurrentStep.stepCount);
            writer.Write(MaxCapacityProperty.CurrentStep.stepCount);
        }


        private static bool Load () {
            var reader = GameDataStorage.GetDataReader(Path);
            if (reader is null)
                return false;

            SpeedProperty.CurrentStep = SpeedProperty.upgradablePropertySteps[reader.ReadInteger()];
            MaxHealthProperty.CurrentStep = MaxHealthProperty.upgradablePropertySteps[reader.ReadInteger()];
            MaxCapacityProperty.CurrentStep = MaxCapacityProperty.upgradablePropertySteps[reader.ReadInteger()];
            return true;
        }

    }

}