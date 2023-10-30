using UnityEngine;

namespace BaseDefense {

    [CreateAssetMenu(fileName = "Upgradable Property", menuName = "Upgradable Property", order = 51)]
    public class UpgradableProperty : ScriptableObject {

        public string viewName;
        public UpgradablePropertyType upgradablePropertyType;
        public UpgradablePropertyStep[] upgradablePropertySteps;
        public UpgradablePropertyStep CurrentStep { get; private set; }


        public void SetNextStep () {
            CurrentStep = upgradablePropertySteps[CurrentStep.stepCount + 1];
        }


        public bool TryGetNextStep (out UpgradablePropertyStep step) {
            CurrentStep ??= upgradablePropertySteps[0];
            var nextStepIndex = CurrentStep.stepCount + 1;

            if (nextStepIndex >= upgradablePropertySteps.Length) {
                step = null;
                return false;
            }

            step = upgradablePropertySteps[nextStepIndex];
            return true;
        }

    }

}