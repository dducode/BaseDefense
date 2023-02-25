using BaseDefense;
using BaseDefense.Properties;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ObjectId))]
    public class ObjectIdProperty : PropertyDrawer
    {
        private int m_objectId;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var objectId = property.FindPropertyRelative("id");

            using var propertyScope = new EditorGUI.PropertyScope(position, label, property);

            var idIntValue = objectId.intValue;
            var rect = EditorGUI.PrefixLabel(position, label);
            
            if (idIntValue == 0)
            {
                idIntValue = Random.Range(int.MinValue, int.MaxValue);
                objectId.intValue = idIntValue;
                objectId.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.IntField(rect, idIntValue);
        }
    }
}
