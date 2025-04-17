using UnityEditor;
using UnityEngine;

namespace com.animationauthoring { 

    [CustomEditor(typeof(AnimationChild))]
    public class AnimationChildEditorScript : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AnimationChild script = (AnimationChild)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("objectID"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ancor"));


            if (script.ancor == Ancor.Right_Hand || script.ancor == Ancor.Left_Hand)
            {
                //enable ancorOffset
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ancorOffset"));

            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}
