using com.animationauthoring;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.animationauthoring
{
    public class NormalizeAnimationSteps : Editor
    {
        private List<Vector3> worldPositions = new List<Vector3>();

        [MenuItem("Tools/NormalizeAnimationSteps")]
        private static void NormalizeAnimationStep()
        {
            Sequence sequence = null;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                // Prefab mode
                sequence = prefabStage.prefabContentsRoot.GetComponent<Sequence>();
            }
            else
            {
                // Scene mode
                sequence = GameObject.FindObjectOfType<Sequence>();
            }

            if (sequence == null)
            {
                Debug.LogWarning("No Sequence component found in the scene or prefab.");
                return;
            }

            NormalizeAnimationSteps editorInstance = new NormalizeAnimationSteps();
            editorInstance.worldPositions.Clear();
            editorInstance.CollectWorldPositions(sequence.transform);
            Debug.Log(editorInstance.worldPositions.Count);
            editorInstance.SetAnimationsStepsToZero(sequence.transform);
            editorInstance.SetWorldPositions(sequence.transform);

            // Mark the scene or prefab as dirty to ensure changes are saved
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
            else
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            // Register the changes with the Undo system
            Undo.RegisterCompleteObjectUndo(sequence.gameObject, "Normalize Animation Steps");
        }

        private void CollectWorldPositions(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.GetComponent<AnimationChild>() != null)
                {
                    // Collect the world positions of all children first
                    worldPositions.Add(child.position);
                }
                CollectWorldPositions(child);
            }
        }

        private void SetAnimationsStepsToZero(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.GetComponent<Animation_Step>() != null)
                {
                    Undo.RecordObject(child, "Set Animation Step to Zero");
                    child.position = Vector3.zero;
                }
                SetAnimationsStepsToZero(child);
            }
        }

        private void SetWorldPositions(Transform parent)
        {
            int index = 0;
            SetWorldPositionsRecursive(parent, ref index);
        }

        private void SetWorldPositionsRecursive(Transform parent, ref int index)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.GetComponent<AnimationChild>() != null)
                {
                    Undo.RecordObject(child, "Set World Position");
                    child.position = worldPositions[index];
                    index++;
                }
                SetWorldPositionsRecursive(child, ref index);
            }
        }
    }
}
