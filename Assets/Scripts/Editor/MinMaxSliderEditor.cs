using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxSliderFloat))]
[CustomPropertyDrawer(typeof(MinMaxSliderInt))]
public class MinMaxSliderEditor : PropertyDrawer
{
    const int LINE_COUNT = 2;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * LINE_COUNT;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var minValueProperty = property.FindPropertyRelative("minValue");
        var maxValueProperty = property.FindPropertyRelative("maxValue");

        var minLimitProperty = property.FindPropertyRelative("minLimit");
        var maxLimitProperty = property.FindPropertyRelative("maxLimit");

        using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
        {
            var sliderRect = EditorGUI.PrefixLabel(position, label);
            sliderRect.height = position.height / LINE_COUNT;

            var valuesRect = sliderRect;
            valuesRect.y += sliderRect.height;
            valuesRect.width /= 2.0f;

            var minValueRect = valuesRect;
            var maxValueRect = valuesRect;
            maxValueRect.x += minValueRect.width;

            if (minValueProperty.propertyType == SerializedPropertyType.Float)
            {
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

                if (EditorGUI.EndChangeCheck())
                {
                    minValueProperty.floatValue = minValueFloat;
                    maxValueProperty.floatValue = maxValueFloat;
                }
            }

            if (minValueProperty.propertyType == SerializedPropertyType.Integer)
            {
                var minValueInt = (float)minValueProperty.intValue;
                var maxValueInt = (float)maxValueProperty.intValue;

                EditorGUI.BeginChangeCheck();

                EditorGUI.MinMaxSlider(
                    sliderRect,
                    ref minValueInt,
                    ref maxValueInt,
                    minLimitProperty.intValue,
                    maxLimitProperty.intValue
                );

                minValueInt = EditorGUI.IntField(minValueRect, (int)minValueInt);
                maxValueInt = EditorGUI.IntField(maxValueRect, (int)maxValueInt);

                if (EditorGUI.EndChangeCheck())
                {
                    minValueProperty.intValue = (int)minValueInt;
                    maxValueProperty.intValue = (int)maxValueInt;
                }
            }
        }
    }
}
