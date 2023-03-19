using BaseDefense.Properties;
using UnityEditor;
using UnityEngine;

namespace Editor {

    [CustomPropertyDrawer(typeof(MinMaxSliderFloat))]
    [CustomPropertyDrawer(typeof(MinMaxSliderInt))]
    public class MinMaxSliderProperty : PropertyDrawer {

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            var minValueProperty = property.FindPropertyRelative("minValue");
            var maxValueProperty = property.FindPropertyRelative("maxValue");

            var minLimitProperty = property.FindPropertyRelative("minLimit");
            var maxLimitProperty = property.FindPropertyRelative("maxLimit");

            using var propertyScope = new EditorGUI.PropertyScope(position, label, property);

            var sliderRect = EditorGUI.PrefixLabel(position, label);
            sliderRect.width /= 2.0f;

            var valuesRect = sliderRect;
            valuesRect.width /= 2.5f;

            var minValueRect = valuesRect;
            var maxValueRect = valuesRect;
            sliderRect.x += valuesRect.width * 1.25f;
            maxValueRect.x += valuesRect.width * 1.5f + sliderRect.width;

            if (minValueProperty.propertyType == SerializedPropertyType.Float) {
                var minValueFloat = minValueProperty.floatValue;
                var maxValueFloat = maxValueProperty.floatValue;

                EditorGUI.BeginChangeCheck();

                EditorGUI.MinMaxSlider(
                    sliderRect,
                    ref minValueFloat,
                    ref maxValueFloat,
                    minLimitProperty.floatValue,
                    maxLimitProperty.floatValue
                );

                minValueFloat = EditorGUI.FloatField(minValueRect, minValueFloat);
                maxValueFloat = EditorGUI.FloatField(maxValueRect, maxValueFloat);

                if (EditorGUI.EndChangeCheck()) {
                    minValueProperty.floatValue = minValueFloat;
                    maxValueProperty.floatValue = maxValueFloat;
                }
            }

            if (minValueProperty.propertyType == SerializedPropertyType.Integer) {
                var minValueInt = (float) minValueProperty.intValue;
                var maxValueInt = (float) maxValueProperty.intValue;

                EditorGUI.BeginChangeCheck();

                EditorGUI.MinMaxSlider(
                    sliderRect,
                    ref minValueInt,
                    ref maxValueInt,
                    minLimitProperty.intValue,
                    maxLimitProperty.intValue
                );

                minValueInt = EditorGUI.IntField(minValueRect, (int) minValueInt);
                maxValueInt = EditorGUI.IntField(maxValueRect, (int) maxValueInt);

                if (EditorGUI.EndChangeCheck()) {
                    minValueProperty.intValue = (int) minValueInt;
                    maxValueProperty.intValue = (int) maxValueInt;
                }
            }
        }

    }

}