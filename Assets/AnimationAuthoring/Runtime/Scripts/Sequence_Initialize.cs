using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCIKonstanz.Colibri.Synchronization;
namespace com.animationauthoring
{
    public class Sequence_Initialize : MonoBehaviour
    {
        private GameObject sceneObjects;
        private Sequence sequence;

        public GameObject Initialize_sceneObjects()
        {
            //create object to hold objects that are animated
            sceneObjects = new GameObject("SceneObjects");
            sceneObjects.transform.SetParent(this.transform);
            sceneObjects.transform.localPosition = new Vector3(0, 0, 0);
            sceneObjects.transform.rotation = new Quaternion(0, 0, 0, 0);

            return sceneObjects;
        }

        /// <summary>
        /// Find all Animation child objects in the given parent
        /// </summary>
        /// <param name="parent">The given parent object</param>
        /// <returns></returns>
        public List<SceneObjectData> FindAnimationChildObjects(GameObject parent)
        {
            var sceneDataObjects = new List<SceneObjectData>();
            sceneDataObjects.Clear();

            AnimationChild[] animationChildren = parent.GetComponentsInChildren<AnimationChild>(true);
            foreach (AnimationChild child in animationChildren)
            {
                sceneDataObjects.Add(new SceneObjectData(child.gameObject, Ancor.World, child.objectID));
            }

            // Return the list of GameObjects with AnimationChild script attached
            return sceneDataObjects;
        }

        /// <summary>
        /// Function to deactivate all children in Prefab of GameObjects with the Animation Step script.
        /// </summary>
        public void DeactivateAllChildren()
        {
            Animation_Step[] foundObjects = GetComponentsInChildren<Animation_Step>();
            foreach (Animation_Step obj in foundObjects)
            {
                DeactivateChildren(obj.gameObject);
            }
        }
        /// <summary>
        /// Function to deactivate all direct children of a given gameobject
        /// </summary>
        /// <param name="parent">The gameobject of which all children will be deactivated</param>
        public void DeactivateChildren(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is null.");
                return;
            }

            foreach (Transform childTransform in parent.transform)
            {
                childTransform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Function that fetches direct children of an Gameobject and instantiates them into the sceneObjects Gameobject
        /// </summary>
        /// <param name="parent">The gameobject the children are copied from </param>
        public void FetchAndCopyChildren(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is null.");
                return;
            }

            Transform parentTransform = parent.transform;

            // Create scene object copies as children of SceneObjects
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                // Instantiate a copy of the child object
                GameObject sceneObjectCopy = Instantiate(child.gameObject, sceneObjects.transform);
                

                AssignAnimationScript(sceneObjectCopy.transform);

                sceneObjectCopy.SetActive(true);
            }
        }
        /// <summary>
        /// Function that assign the AnimationScript to a Gameobject and of its children
        /// </summary>
        /// <param name="parent">The transform of a Gameobject</param>
        void AssignAnimationScript(Transform parent)
        {
            // Assign the script to the parent object
            parent.gameObject.AddComponent<Animation>();
            sequence = this.transform.GetComponent<Sequence>();
            if (sequence.networking) {
                var syncTransform = parent.gameObject.AddComponent<SyncTransform>();
                syncTransform.SyncActive = false;
            }
            // We have to also assign AnimationChild on Runtime, as the Start-State Objects dont have it by default
            if (parent.GetComponent<AnimationChild>() == null)
            {
                parent.gameObject.AddComponent<AnimationChild>();
            }

            // Recursively assign the script to all child objects
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                AssignAnimationScript(child);
            }
        }


    }
}
