//using Codice.Client.BaseCommands;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace com.animationauthoring
{
    /// <summary>
    /// Class that changes the Base_script inspector view, adds and removes the ancorOffset variable and adds a comprehensive view of children with the animation_step script
    /// </summary>
    [CustomEditor(typeof(Animation_Step))]
    public class AnimationScriptEditor : Editor
    {

        ITriggers triggerScript;

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            Animation_Step script = (Animation_Step)target;

            if (triggerScript == null)
            {
                MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
                List<ITriggers> triggerScripts = new List<ITriggers>();
                for (int i = 0; i < allScripts.Length; i++)
                {
                    if (allScripts[i] is ITriggers)
                        triggerScripts.Add(allScripts[i] as ITriggers);
                }
                if (triggerScripts.Count == 1)
                {
                    triggerScript = triggerScripts[0];
                }
                else if (triggerScripts.Count > 1)
                {
                    Debug.LogError("Multiple trigger scripts found, please make sure only one Gameobject in the Scene holds a script that implements the ITrigger interface");
                }
                else
                {
                    Debug.LogError("No trigger script found, please make sure one one Gameobject in the Scene holds a script that implements the ITrigger interface");
                }
            }

            // Get the list of triggers
            List<string> triggers = triggerScript.trigger;
            // Find the current index of the trigger
            int currentIndex = triggers.IndexOf(script.trigger);

            if (currentIndex < 0)
            {
                // The trigger was not found in the list
                // Set currentIndex to 0 or display an error message
                currentIndex = 0;
                Debug.LogWarning("Trigger not found in the list.");
            }

            // Display the popup and get the selected index
            int selectedIndex = EditorGUILayout.Popup("Trigger", currentIndex, triggers.ToArray());

            // Check if the list contains any elements and if the selected index is valid
            if (triggers.Count > 0 && selectedIndex < triggers.Count)
            {
                // Assign the selected trigger to the script
                script.trigger = triggers[selectedIndex];
            }
            else
            {
                // The list is empty or the selected index is invalid
                // Display an error message
                Debug.LogError("No triggers available or invalid selection.");
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animationStyle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animationDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doAnimate"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("ancor"));
            //check if ancor is either right hand or left hand
            /*if (script.ancor == Ancor.Right_Hand || script.ancor == Ancor.Left_Hand)
            {
                //enable ancorOffset
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ancorOffset"));
            }*/
            serializedObject.ApplyModifiedProperties();
            AttachScriptToChildren();
            DisplayAnimationChildProperties(script);
        }
        private void DisplayAnimationChildProperties(Animation_Step animationStep)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("AnimationChild Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            foreach (AnimationChild childScript in animationStep.GetComponentsInChildren<AnimationChild>())
            {
                EditorGUILayout.LabelField(childScript.gameObject.name, EditorStyles.boldLabel);

                EditorGUI.indentLevel++;
                SerializedObject childSerializedObject = new SerializedObject(childScript);
                SerializedProperty ancorProperty = childSerializedObject.FindProperty("ancor");
                SerializedProperty ancorOffsetProperty = childSerializedObject.FindProperty("ancorOffset");

                EditorGUILayout.PropertyField(ancorProperty);
                if (childScript.ancor == Ancor.Right_Hand || childScript.ancor == Ancor.Left_Hand)
                {
                    //enable ancorOffset
                    EditorGUILayout.PropertyField(ancorOffsetProperty);
                }

                EditorGUI.indentLevel--;
                childSerializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;
        }

        private void AttachScriptToChildren()
        {
            Animation_Step animationStep = (Animation_Step)target;

            // Iterate through child GameObjects and attach your script if it's not already attached.
            foreach (Transform child in animationStep.transform)
            {
                if (child.GetComponent<AnimationChild>() == null)
                {
                    child.gameObject.AddComponent<AnimationChild>();
                }
                if (child.childCount != 0)
                {
                    AttachScriptToChildren(child.gameObject);
                }
            }
        }

        private void AttachScriptToChildren(GameObject gameObject)
        {
            // Iterate through child GameObjects and attach your script if it's not already attached.
            foreach (Transform child in gameObject.transform)
            {
                if (child.GetComponent<AnimationChild>() == null)
                {
                    child.gameObject.AddComponent<AnimationChild>();
                }
                if (child.childCount != 0)
                {
                    AttachScriptToChildren(child.gameObject);
                }
            }
        }
    }
}
