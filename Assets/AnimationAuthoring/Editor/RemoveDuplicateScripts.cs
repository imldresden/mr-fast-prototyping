using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace com.animationauthoring
{
    public class RemoveDuplicateScripts : EditorWindow
    {
        [MenuItem("Tools/Remove Duplicate Scripts")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(RemoveDuplicateScripts));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Remove Duplicate Scripts"))
            {
                RemoveDuplicates();
            }
        }

        private void RemoveDuplicates()
        {
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject go in gameObjects)
            {
                Component[] components = go.GetComponents<Component>();

                // Create a dictionary to store component types and their counts.
                var componentCounts = new Dictionary<Type, int>();

                foreach (Component component in components)
                {
                    Type type = component.GetType();

                    if (componentCounts.ContainsKey(type))
                    {
                        // A component of this type already exists on the GameObject.
                        // Remove the excess component.
                        DestroyImmediate(component);
                    }
                    else
                    {
                        // First occurrence of this component type.
                        componentCounts[type] = 1;
                    }
                }
            }

            Debug.Log("Duplicate scripts removed.");
        }
    }
}