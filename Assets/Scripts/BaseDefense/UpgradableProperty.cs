using System.IO;
using BaseDefense.SaveSystem;
using UnityEngine;


namespace BaseDefense {

    [CreateAssetMenu(fileName = "Upgradable Property", menuName = "Upgradable Property", order = 51)]
    public class UpgradableProperty : ScriptableObject {

        public UpgradablePropertyType upgradablePropertyType;
        public UpgradablePropertyStep[] upgradablePropertySteps;
        public UpgradablePropertyStep CurrentStep { get; set; }


        public void SetNextStep () {
            CurrentStep = upgradablePropertySteps[CurrentStep.stepCount + 1];
        }


        public UpgradablePropertyStep GetNextStep () {
            return upgradablePropertySteps[CurrentStep.stepCount + 1];
        }

    }

}