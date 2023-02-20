using UnityEditor;

namespace BaseDefense
{
    [CustomEditor(typeof(Upgrades))]
    public class InspectorForUpgrades : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Значения ниже устанавливают максимально возможные свойства игрока при прокачке",
                MessageType.Info
            );
            DrawDefaultInspector();
        }
    }
}


