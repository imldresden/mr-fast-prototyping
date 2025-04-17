using UnityEngine;
using UnityEditor;

namespace com.animationauthoring
{
    /// <summary>
    /// Custom property drawer for the ConditionalShow attribute. This drawer controls the visibility
    /// of serialized fields in the Unity inspector based on a specified condition.
    /// </summary>
    [CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
    public class ConditionalShowDrawer : PropertyDrawer
    {
        /// <summary>
        /// Override for OnGUI method, responsible for rendering the property field in the Unity inspector.
        /// </summary>
        /// <param name="position">The position and dimensions of the property field.</param>
        /// <param name="property">The serialized property being drawn.</param>
        /// <param name="label">The label associated with the property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalShowAttribute condAttribute = attribute as ConditionalShowAttribute;
            SerializedProperty conditionalField = property.serializedObject.FindProperty(condAttribute.conditionalFieldName);

            if (conditionalField != null)
            {
                bool show = CheckCondition(conditionalField, condAttribute.expectedValue);
                if (show)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        /// <summary>
        /// Checks the specified condition against the value of the serialized property.
        /// </summary>
        /// <param name="property">The serialized property to check.</param>
        /// <param name="expectedValue">The expected value to compare the property against.</param>
        /// <returns>True if the condition is met; otherwise, false.</returns>
        private bool CheckCondition(SerializedProperty property, object expectedValue)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex == (int)expectedValue;
                case SerializedPropertyType.Integer:
                    return property.intValue == (int)expectedValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue == (bool)expectedValue;
                default:
                    return true;
            }
        }
    }
}