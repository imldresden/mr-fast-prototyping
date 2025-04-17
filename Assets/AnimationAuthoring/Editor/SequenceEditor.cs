using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;


namespace com.animationauthoring
{
    [CustomEditor(typeof(Sequence))]
    public class SequenceEditor : Editor
    {
        private Dictionary<Animation_Step, bool> showChildrenPropertiesDict = new Dictionary<Animation_Step, bool>();

        ITriggers triggerScript;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (triggerScript == null)
            {
                // Check if we are in prefab mode
                //if (PrefabStageUtility.GetCurrentPrefabStage() != null)

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
                    //could not find trigger script, if we are in prefab mode search for it another way
                    Sequence seqScript = (Sequence)target;
                    triggerScript = seqScript.GetComponent<ITriggers>();

                    if (triggerScript == null)
                    {

                        Debug.LogError("No trigger script found, please make sure one one Gameobject in the Scene holds a script that implements the ITrigger interface. " +
                            "If you are in Prefab mode make sure a trigger script implementing ITrigger is attached to the Gameobject that the Sequence Script is attached to.");
                    }
                }
            }

            Sequence script = (Sequence)target;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startState"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("endState"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("keybind"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("loop"));

            // Iterate through the direct children of the GameObject
            for (int i = 0; i < script.transform.childCount; i++)
            {
                Animation_Step animationStep = script.transform.GetChild(i).GetComponent<Animation_Step>();
                if (animationStep != null)
                {
                    //also attach scripts to children of Start Step
                    AttachScriptToChildren(animationStep.gameObject);

                        EditorGUILayout.Space(); // Add some spacing between Animation_Step components
                    if (animationStep.gameObject == script.startState)
                    {
                           EditorGUILayout.LabelField("Start State (condition to end loop)", EditorStyles.boldLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(animationStep.gameObject.name, EditorStyles.boldLabel);
                    }

                    // Toggle to show/hide children properties



                    EditorGUI.indentLevel++;

                        SerializedObject animationStepSerializedObject = new SerializedObject(animationStep);



                        // Get the list of triggers
                        List<string> triggers = triggerScript.trigger;
                        // Find the current index of the trigger
                        SerializedProperty triggerProperty = animationStepSerializedObject.FindProperty("trigger");
                        int currentIndex = triggers.IndexOf(triggerProperty.stringValue);
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
                            triggerProperty.stringValue = triggers[selectedIndex];
                        }
                        else
                        {
                            // The list is empty or the selected index is invalid
                            // Display an error message
                            Debug.LogError("No triggers available or invalid selection.");
                        }


                        SerializedProperty animationStyleProperty = animationStepSerializedObject.FindProperty("animationStyle");
                        SerializedProperty animationDurationProperty = animationStepSerializedObject.FindProperty("animationDuration");
                        SerializedProperty doAnimateProperty = animationStepSerializedObject.FindProperty("doAnimate");

                        EditorGUILayout.PropertyField(animationStyleProperty);
                        EditorGUILayout.PropertyField(animationDurationProperty);
                        EditorGUILayout.PropertyField(doAnimateProperty);


                        animationStepSerializedObject.ApplyModifiedProperties();

                        bool showChildrenProperties = GetShowChildrenProperties(animationStep);
                        showChildrenProperties = EditorGUILayout.Foldout(showChildrenProperties, "Show Individual Object Ancors", true);
                        SetShowChildrenProperties(animationStep, showChildrenProperties);
                        if (showChildrenProperties)
                        {
                            // Display ancors and ancorOffsets for AnimationChild scripts
                            DisplayAnimationChildProperties(animationStep);
                        }
                        EditorGUI.indentLevel--;

                    
                }
                else
                {
                    Debug.LogWarning("Animation_Step component not found on child GameObject with the name '"+ script.transform.GetChild(i).name + 
                        "'. This GameObject will be ignored for animation.");
                }
            }

            serializedObject.ApplyModifiedProperties();
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

        private bool GetShowChildrenProperties(Animation_Step animationStep)
        {
            if (showChildrenPropertiesDict.ContainsKey(animationStep))
            {
                return showChildrenPropertiesDict[animationStep];
            }
            return false; // Default to showing properties if not found
        }

        private void SetShowChildrenProperties(Animation_Step animationStep, bool show)
        {
            if (showChildrenPropertiesDict.ContainsKey(animationStep))
            {
                showChildrenPropertiesDict[animationStep] = show;
            }
            else
            {
                showChildrenPropertiesDict.Add(animationStep, show);
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