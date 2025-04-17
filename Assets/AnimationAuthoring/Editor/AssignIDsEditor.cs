using com.animationauthoring;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AssignIDsEditor : EditorWindow
{
    private GameObject prefab;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Assign IDs")]
    public static void ShowWindow()
    {
        GetWindow<AssignIDsEditor>("Assign IDs");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Assign IDs Tool", EditorStyles.boldLabel);

        string explanationText = "This tool assigns unique IDs to all AnimationChild components in the scene or a selected prefab. " +
            "The IDs are used to identify the objects in the sequence and are required for the sequence to function correctly." +
            "Adding these IDs for newly created Ids can be tedious, but be wary that this script overwrites all IDs in a scene or a prefab.";

        EditorGUILayout.LabelField(explanationText, EditorStyles.wordWrappedLabel, GUILayout.ExpandWidth(true));

        GUILayout.Space(20);

        GUILayout.Label("Assign IDs in Scene", EditorStyles.boldLabel);
        if (GUILayout.Button("Assign IDs in Scene"))
        {
            AssignIDsInScene();
        }

        GUILayout.Space(20);

        GUILayout.Label("Assign IDs to Prefab", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (GUILayout.Button("Assign IDs to Prefab"))
        {
            AssignIDsToPrefab();
        }

        EditorGUILayout.EndScrollView();
    }

    private void AssignIDsInScene()
    {
        Sequence sequence = FindObjectOfType<Sequence>();
        if (sequence == null)
        {
            Debug.LogError("No Sequence script found in the active scene.");
            return;
        }

        foreach (Transform child in sequence.transform)
        {
            int idCounter = 1;

            AssignIDsRecursively(child, ref idCounter);

        }
    }

    private void AssignIDsToPrefab()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab selected.");
            return;
        }

        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (prefabInstance != null)
        {
            foreach (Transform child in prefabInstance.transform)
            {
                int idCounter = 1;

                AssignIDsRecursively(child, ref idCounter);
            }

            // Save the changes to the prefab asset
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            DestroyImmediate(prefabInstance);
            Debug.Log("IDs assigned to prefab successfully.");
        }
        else
        {
            Debug.LogError("Failed to instantiate prefab.");
        }
    }

    private void AssignIDsRecursively(Transform parent, ref int idCounter)
    {
        foreach (Transform child in parent)
        {
            AnimationChild animationChild = child.GetComponent<AnimationChild>();
            if (animationChild != null)
            {
                animationChild.objectID = idCounter;
                idCounter++;
                AssignIDsRecursively(child, ref idCounter);
            }
        }
    }
}

