using HCIKonstanz.Colibri.Synchronization;
using Microsoft.MixedReality.Toolkit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling;
using UnityEngine;
using UnityEngine.Networking.Types;
using static UnityEngine.GraphicsBuffer;


namespace com.animationauthoring
{
    public class Sequence : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField]
        [Tooltip("The Start state, holding the initial position of all objects")]
        public GameObject startState;

        [SerializeField]
        [Tooltip("The final state, that ends the animation")]
        public GameObject endState;

        [SerializeField]
        [Tooltip("The key that stops all tracking to certain ancors temporarily")]
        public string keybind = "m";

        [SerializeField]
        [Tooltip("Whether the standard trigger should be used or not")]
        public bool loop = true;


        private List<Vector3> worldPositions = new List<Vector3>();

        // internal variables used for handling animation
        private GameObject sceneObjects;
        private List<SceneObjectData> sceneDataObjects = new List<SceneObjectData>();


        private int animationsRunning;

        private bool observerCanContinue = false;
        private bool keyIsPressed = false;
        //Dictionary of Gameobjects and Animation
        private Dictionary<Animation, GameObject> transformParentDict = new Dictionary<Animation, GameObject>();
        private ColibriNetworkManager networkLobby;
        public bool networking = true;
        ITriggers triggers;


        Sequence_Initialize sequenceInitialize;

        // Start is called before the first frame update
        void Start()
        {
            // check if Sequence_Initialize script is attached to the same GameObject and if not attach it
            sequenceInitialize = GetComponent<Sequence_Initialize>();
            if (sequenceInitialize == null)
            {
                sequenceInitialize = gameObject.AddComponent<Sequence_Initialize>();
            }
            MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
            List<ITriggers> triggerScripts = new List<ITriggers>();
            for (int i = 0; i < allScripts.Length; i++)
            {
                if (allScripts[i] is ITriggers)
                    triggerScripts.Add(allScripts[i] as ITriggers);
            }
            if (triggerScripts.Count == 1)
            {
                triggers = triggerScripts[0];
            }
            else if (triggerScripts.Count > 1)
            {
                Debug.LogError("Multiple trigger scripts found, please make sure only one Gameobject in the Scene holds a script that implements the ITrigger interface");
            }
            else
            {
                Debug.LogError("No trigger script found, please make sure one one Gameobject in the Scene holds a script that implements the ITrigger interface");
            }

            networkLobby = FindObjectOfType<ColibriNetworkManager>();
            if (networkLobby == null)
            {
                networking = false;
                Debug.LogWarning("No ColibriNetworkManager found in the scene. Sequence is started without networking");
            }

            //SetupRPCListeners();
            Initialize();

        }

        private void OnEnable()
        {
            // Register the RPC listeners
            if (networking)
                SetupRPCListeners();
        }
        private void OnDisable()
        {
            // Unregister the RPC listeners
            if (networking)
                RemoveListeners();
        }

        public void SetupRPCListeners()
        {
            Sync.Receive("DestroySequence", (bool unrelevant) => {
                DestroySequence(unrelevant);
            });
            Sync.Receive("CanObserverContinue", (bool canObserverContinue) =>
            {
                CanObserverContinue(canObserverContinue);
            });

        }
        public void RemoveListeners()
        {
            Sync.Unregister("DestroySequence", DestroySequence);
            Sync.Unregister("CanObserverContinue", CanObserverContinue);
        }

        public void DestroySequence(bool unrelevant)
        {
            //Sync.Unregister("DestroySequence", DestroySequence);
            StopAllCoroutines();
            //destroy all objects in transformParentDict as we dont know where it is at moment of this call
            foreach (KeyValuePair<Animation, GameObject> entry in transformParentDict)
            {
                Destroy(entry.Value);
            }
            // Destroy all descendants of this GameObject
            Destroy(this.gameObject);
            Debug.Log("DestroySequence complete");
        }

        /// <summary>
        /// Stops all coroutines that are currently running to cleanly delete the object
        /// </summary>
        public void StopCoroutines()
        {
            StopAllCoroutines();

        }

        /// <summary>
        /// Sets up prefab components for animation and interaction, function is called by Networking scripts
        /// </summary>
        public void Initialize()
        {

            CheckAnimationSteps();

            sceneObjects = sequenceInitialize.Initialize_sceneObjects();

            // Deactivate all objects in the prefab
            sequenceInitialize.DeactivateChildren(startState);
            sequenceInitialize.DeactivateAllChildren();

            //Start actual animation here
            // Fetch children from "Start State" and create scene object copies
            sequenceInitialize.FetchAndCopyChildren(startState);
            sceneDataObjects = sequenceInitialize.FindAnimationChildObjects(sceneObjects);
            //start a Coroutine with the sequence

            StartCoroutine(Sequence_Coroutine());

            sceneObjects.AddComponent<ObjectManipulator>();
        }
        /// <summary>
        /// Checks animationsteps localposition for unexpected behaviour and issues a warning if unexpected behavior could occur
        /// </summary>
        private void CheckAnimationSteps()
        {
            //check if one or more animation steps has a world position other than (0,0,0)
            Animation_Step[] foundObjects = GetComponentsInChildren<Animation_Step>();
            foreach (Animation_Step obj in foundObjects)
            {
                if (obj.gameObject.transform.localPosition != Vector3.zero)
                {
                    Debug.LogWarning("Animation Step " + obj.gameObject.name + " has a world position other than (0,0,0). This might cause unexpected behaviour within the animation. " +
                        "To normalize the animationstep position use Tools > NormalizeAnimationSteps");
                }
            }
        }
            /// <summary>
            /// Update method is used to check if the key is pressed and if so, it will stop or resume the animation sequence
            /// </summary>
            private void Update()
        {
            if (Input.GetKey(keybind))
            {
                if (!keyIsPressed)
                {
                    // Key is being pressed for the first time in this frame
                    keyIsPressed = true;
                }
            }
            else
            {
                // Key is not pressed
                keyIsPressed = false;
            }


        }
        /// <summary>
        /// Function called by networking to synchronize the Sequence function;
        /// </summary>
        /// <param name="canObserverContinue"> Boolean that is used to synchronize sequence function</param>

        private void CanObserverContinue(bool canObserverContinue)
        {
            observerCanContinue = canObserverContinue;
        }

        /// <summary>
        /// Main Coroutine that executes animation for each individual step, waits for trigger etc. 
        /// </summary>
        /// <returns>When sequence is completed</returns>
        private IEnumerator Sequence_Coroutine()
        {
            Animation_Step[] foundObjects = GetComponentsInChildren<Animation_Step>();
            bool firstLoop = true;
            while (loop || firstLoop)
            {
                if (!firstLoop)
                { 
                    RecreateStartState();
                }

                foreach (Animation_Step obj in foundObjects)
                {
                    if ((obj.gameObject != endState) && (obj.gameObject != startState) && (obj.doAnimate))
                    {
                        yield return StartCoroutine(AnimateObject(obj));
                    }
                }

                if (endState.GetComponent<Animation_Step>().doAnimate)
                {
                    yield return StartCoroutine(AnimateObject(endState.GetComponent<Animation_Step>()));
                }
                if (startState.GetComponent<Animation_Step>() != null)
                { 
                    if (startState.GetComponent<Animation_Step>().doAnimate)
                    {
                        yield return StartCoroutine(AnimateObject(startState.GetComponent<Animation_Step>()));
                    }
                }
                else
                {
                    Debug.LogWarning("Start State does not have an Animation_Step component attached");
                }
                firstLoop = false;
                if (loop)
                {
                    Debug.Log("Starting a new loop.");
                }
            }
        }
        /// <summary>
        /// Recreates start state for the actor case
        /// </summary>
        /// <returns></returns>
        private void RecreateStartState()
        {
            for (int i = sceneDataObjects.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(sceneDataObjects[i].gameObj);
            }
            Debug.Log("Copied Start State");
            GameObject.Destroy(sceneObjects);
            sceneObjects = sequenceInitialize.Initialize_sceneObjects();
            // Debug log the children Gameobjects in sceneObjects
            Transform[] temp = sceneObjects.GetComponentsInChildren<Transform>();
            sequenceInitialize.FetchAndCopyChildren(startState);
            sceneDataObjects = sequenceInitialize.FindAnimationChildObjects(sceneObjects);
            /*animationsRunning = 0;
            MoveChildrenRecursively(startState.transform, FetchStateChildTransforms(startState), endState.GetComponent<Animation_Step>());
            yield return StartCoroutine(WaitForAnimationsToComplete());
            Debug.Log("Recreated Start State");*/
        }


        /// <summary>
        /// Observer waits for signal from Actor to continue
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForActor()
        {
            while (!observerCanContinue)
            {
                yield return null;
            }
            observerCanContinue = false;

        }



        /// <summary>
        /// Animate the given object based on context variables and networking
        /// </summary>
        /// <param name="obj">Animation step object that is to be animated</param>
        /// <returns></returns>
        private IEnumerator AnimateObject(Animation_Step obj)
        {
            if (networking) { 
                    observerCanContinue = false;
                    yield return StartCoroutine(WaitForTriggerNetworked(obj.trigger));
                    //if observercancontinue is false (i.e. we had a succesful trigger), we need to send a signal to the observer that we are done
                    if (observerCanContinue == false && networking)
                    {
                        Sync.Send("CanObserverContinue", true);
                    }
            }
            //no networking path
            else
            {
                yield return StartCoroutine(WaitForTrigger(obj.trigger));
            }
            //Debug.Log("Animating: " + obj.gameObject.name);
            yield return StartCoroutine(FetchAndMoveChildren(obj.gameObject));
        }

        /// <summary>
        /// Wait for the given trigger to be called
        /// </summary>
        /// <param name="trigger">Trigger that will progress the sequence</param>
        /// <returns></returns>
        /// 
        private IEnumerator WaitForTriggerNetworked(String trigger)
        {
            while (true)
            {
                if (triggers.EvalAndCallTrigger(trigger))
                {
                    if (trigger != "None")
                    {
                        //menuManager.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                        //We have to set the active player locally and in the network
                        MenuManager menuManager = FindObjectOfType<MenuManager>();
                        menuManager.SetActivePlayer(networkLobby.self.Id);
                        Sync.Send("SetActivePlayer", networkLobby.self.Id);
                    }
                    break;
                }
                else if (observerCanContinue)
                {
                    break;
                }
                yield return null;
            }
        }

        private IEnumerator WaitForTrigger(String trigger)
        {
            while (true)
            {
                if (triggers.EvalAndCallTrigger(trigger))
                {
                    break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Wait for all animations to complete
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForAnimationsToComplete()
        {
            while (animationsRunning > 0)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Coroutine that animates the objects in sceneObjects for a given animation step. The given Gameobject should be an animation step.
        /// </summary>
        /// <param name="parent">The Gameobject holding the children Gameobjects that the sceneObjects will be animated to</param>
        /// <returns>When animation is complete</returns>
        private IEnumerator FetchAndMoveChildren(GameObject parent)
        {
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is null.");
                yield return null;
            }

            // Fetch transforms from given parent state
            Animation_Step animationStep = parent.GetComponent<Animation_Step>();

            if (animationStep == null)
            {
                Debug.LogError("Animation_Step component not found on the given Gameobject! Make sure all direct children of the object with base_script in your prefab have a Animation_step component, except for the Start_state");
            }
            // Counter to track the number of animations running
            animationsRunning = 0;

            MoveChildrenIteratively(parent, animationStep);

            // Wait for all animations to complete
            while (animationsRunning > 0)
            {
                yield return null;
            }
        }

        /// <summary>
        /// alternative function to MoveChildrenRecuresively, that moves all children of a given parent to the position of the children of the endstate
        /// We fetch the children of the targetParent here but of the sceneObjects, since the sceneObjects migth be scattered across the ancors
        /// </summary>
        /// <param name="targetParent">The parent of the current animationStep</param>
        /// <param name="animationStep">The Animation_Step data used for animations.</param>
        private void MoveChildrenIteratively(GameObject targetParent, Animation_Step animationStep)
        {
            List<SceneObjectData> targetChildren = sequenceInitialize.FindAnimationChildObjects(targetParent);

            List<Transform> childTransforms = new List<Transform>();

            foreach (SceneObjectData childTransform in targetChildren)
            {
                childTransforms.Add(childTransform.gameObj.transform);
            }

            //Go through children in the current animation step
            for (int i = 0; i < sceneDataObjects.Count; i++)
            {
                SceneObjectData currentSceneObjectData = sceneDataObjects[i];
                Transform child = currentSceneObjectData.gameObj.transform;
                Animation animation = child.GetComponent<Animation>();
                //find the target object in childtransforms that has the same objectID as the current object
                if (animation != null)
                {
                    AnimationChild current = child.GetComponent<AnimationChild>();
                    //find the target object in childtransforms that has the same objectID as the current object
                    AnimationChild target = null;
                    for (int j = 0; j < childTransforms.Count; j++)
                    {
                        if (childTransforms[j].GetComponent<AnimationChild>().objectID == currentSceneObjectData.id)
                        {
                            target = childTransforms[j].GetComponent<AnimationChild>();
                            break;
                        }
                    }                    
                    //if we cant find a target delete the object 
                    if (target == null)
                    {
                        sceneDataObjects.Remove(currentSceneObjectData);
                        GameObject.Destroy(current.gameObject);      
                        Debug.Log("Deleted object with ID: " + currentSceneObjectData.id);
                    }
                    else { 
                    // Increment the counter before starting the animation
                    animationsRunning++;
                        // Start the animation coroutine
                        //only move object if it should be moved, i.e. if the ancor is world, if the ancor is something else it is being tracked
                        if (current.ancor != target.ancor && current.ancor == Ancor.World)
                        {
                            StartCoroutine(AnimateWorldToTracked(child, current, target, animationStep, childTransforms[i], animation));

                        }
                        else if (current.ancor != target.ancor && target.ancor == Ancor.World)
                        {
                            //Tracked To World
                            AnimateTrackedToWorld(child, current, target, animationStep, childTransforms[i], animation);
                        }
                        else if (current.ancor == Ancor.World)
                        {
                            //World to World
                            StartCoroutine(AnimateWorldToWorld(child, current, target, animationStep, childTransforms[i], animation));
                        }
                        else
                        {
                            animationsRunning--;
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Animation_Step component is missing on a child object.");
                }
       
            }
            //create the new objects that first appear in this animation step, do this at the end, they do not need to be animated
            for (int i = 0; i < childTransforms.Count; i++)
            {
                AnimationChild target = childTransforms[i].GetComponent<AnimationChild>();
                SceneObjectData currentSceneObject = null;
                bool found = false;
                foreach (SceneObjectData sceneObject in sceneDataObjects)
                {
                    if (sceneObject.id == target.objectID) {
                        currentSceneObject = sceneObject;
                        found = true;
                    }
                }
                //SceneObjectData currentSceneObject = sceneDataObjects.Find(x => x.id == target.objectID);
                //if null then the target object id is not present in sceneObjects, hence we have to create it
                if (!found)
                {
                    //create new object at the position of the target object
                    var newObject = GameObject.Instantiate(childTransforms[i].gameObject, sceneObjects.transform);
                    new SceneObjectData(newObject, target.ancor, target.objectID);
                    newObject.SetActive(true);
                    sceneDataObjects.Add(new SceneObjectData(newObject, target.ancor, target.objectID));
                }
            }
        }


        /// <summary>
        /// Recursively moves through the child hierarchy of a given parent, triggering animations
        /// on corresponding child objects based on provided Animation_Step data.
        /// </summary>
        /// <param name="parent">The Gameobject Transform whose children will be traversed.</param>
        /// <param name="childTransforms">A list of child Transform objects corresponding to the target animation step that should be moved.</param>
        /// <param name="animationStep">The Animation_Step data used for animations.</param>
        private void MoveChildrenRecursively(Transform parent, List<Transform> childTransforms, Animation_Step animationStep)
        {
            if (parent.childCount == 0)
            {
                return;
            }
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Animation animation = child.GetComponent<Animation>();

                if (animation != null)
                {
                    if (childTransforms.Count > i)
                    {
                        animationsRunning++;
                        AnimationChild current = child.GetComponent<AnimationChild>();
                        AnimationChild target = childTransforms[i].GetComponent<AnimationChild>();

                        if (current.ancor != target.ancor)
                        {
                            if (current.ancor == Ancor.World)
                            {
                                StartCoroutine(AnimateWorldToTracked(child, current, target, animationStep, childTransforms[i], animation));
                            }
                            else if (target.ancor == Ancor.World)
                            {
                                AnimateTrackedToWorld(child, current, target, animationStep, childTransforms[i], animation);
                            }
                        }
                        else if (current.ancor == Ancor.World)
                        {
                            StartCoroutine(AnimateWorldToWorld(child, current, target, animationStep, childTransforms[i], animation));
                        }
                        else
                        {
                            Debug.Log("Tracked to Tracked");
                            animationsRunning--;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Not enough transforms in 'End State' to match the number of children in 'Start State'.");
                    }
                }
                else
                {
                    Debug.LogWarning("Animation_Step component is missing on a child object.");
                }

                MoveChildrenRecursively(child, FetchStateChildTransforms(childTransforms[i].gameObject), animationStep);
            }
        }
        /// <summary>
        /// Returns transforms of children for a given Gameobject in a list.
        /// </summary>
        /// <param name="parent">The Gameobject, which childrens transforms are returned</param>
        /// <returns>List of Transforms</returns>
        private List<Transform> FetchStateChildTransforms(GameObject parent)
        {
            if (endState == null)
            {
                Debug.LogError("'End State' GameObject is null.");
                return null;
            }

            Transform parentTransform = parent.transform;

            // Clear the list before fetching new transforms
            List<Transform> childTransforms = new List<Transform>();


            // Fetch transforms from "End State" and store them in the list
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform childTransform = parentTransform.GetChild(i);
                childTransforms.Add(childTransform);
            }
            return childTransforms;
        }

        private IEnumerator AnimateWorldToTracked(Transform child, AnimationChild current, AnimationChild target, Animation_Step animationStep, Transform childTransform, Animation animation)
        {
            //Debug.Log("World to Tracked");
            current.oldPosition = childTransform.position;
            current.ancor = target.ancor;
            //Debug.Log("anchor offset: " + target.ancorOffset);
            Vector3 targetLocalPosition = childTransform.localPosition + target.ancorOffset;
            Quaternion targetLocalRotation = current.rotationOffset;
            Vector3 targetLocalScale = childTransform.localScale;
            Debug.Log("Trying to animate and change my parent with target Ancor: " + target.ancor);
            // create new parent for the object that holds transform data
            StartCoroutine(AnimateAndDecrementCoroutine(animation, targetLocalPosition, targetLocalRotation, targetLocalScale, animationStep.animationDuration, animationStep.animationStyle, AncorChange.WorldToTracked, target.ancor));
            child.GetComponent<SyncTransform>().SyncActive = true;
            yield return null;
            //sync newly created parent object via networking

        }

        private void AnimateTrackedToWorld(Transform child, AnimationChild current, AnimationChild target, Animation_Step animationStep, Transform childTransform, Animation animation)
        {
            //Debug.Log("Tracked to World");
            current.ancorOffset = current.oldPosition - current.transform.position;
            current.rotationOffset = current.transform.localRotation;
            current.ancor = target.ancor;
            Vector3 targetLocalPosition = childTransform.localPosition - current.ancorOffset;
            Quaternion targetLocalRotation = current.rotationOffset * Quaternion.Inverse(current.transform.rotation);
            Vector3 targetLocalScale = childTransform.localScale;
            animationsRunning--;
            //sceneObjects.transform.SetParent(transform, true);
            transformParentDict[animation].transform.SetParent(sceneObjects.transform, true);
            //child.SetParent(sceneObjects.transform, true);
            child.GetComponent<SyncTransform>().SyncActive = false;

        }

        private IEnumerator AnimateWorldToWorld(Transform child, AnimationChild current, AnimationChild target, Animation_Step animationStep, Transform childTransform, Animation animation)
        {
            //Debug.Log("World to World");
            Vector3 targetLocalPosition = childTransform.localPosition;
            Quaternion targetLocalRotation = childTransform.localRotation;
            Vector3 targetLocalScale = childTransform.localScale;
            StartCoroutine(AnimateAndDecrementCoroutine(animation, targetLocalPosition, targetLocalRotation, targetLocalScale, animationStep.animationDuration, animationStep.animationStyle, AncorChange.None));
            current.oldPosition = current.transform.position;
            yield return null;
        }

        /// <summary>
        /// Coroutine to keep track of multiple animation that may occur in parallel during an animation step. Decreases global counter.
        /// </summary>
        /// <param name="animation">The animation script object of the Gameobject that is going to be moved</param>
        /// <param name="targetTransform">The transform the Gameobject is moved to</param>
        /// <param name="animationDuration">The length of the duration in seconds</param>
        /// <param name="animationStyle">The style of the animation</param>
        /// <returns>When the started animation finishes</returns>
        private IEnumerator AnimateAndDecrementCoroutine(Animation animation, Vector3 targetLocalPosition, Quaternion targetLocalRotation, Vector3 targetLocalScale, float animationDuration, AnimationStyle animationStyle, AncorChange ancorChange, Ancor trackedAncor = Ancor.World)
        {
            animation.MoveObject(targetLocalPosition, targetLocalRotation, targetLocalScale, animationDuration, animationStyle);
            yield return new WaitUntil(() => animation.IsAnimationComplete(targetLocalPosition));
            switch (ancorChange)
            {
                case AncorChange.WorldToTracked:
                    //sceneObjects.transform.parent = AnchorManager.Instance.AnchorToTransform(trackedAncor);
                    GameObject parent;
                    if (!(transformParentDict.TryGetValue(animation, out parent)))
                    {
                        parent = new GameObject("Parent");
                        transformParentDict.Add(animation, parent);

                        parent.transform.position = sceneObjects.transform.position;
                        parent.transform.rotation = sceneObjects.transform.rotation;
                        parent.transform.localScale = sceneObjects.transform.localScale;

                    }
                    parent.transform.SetParent(AnchorManager.Instance.AnchorToTransform(trackedAncor), true);
                    //if active player and other players present wait for other players to arrive here
                    MenuManager menuManager = FindObjectOfType<MenuManager>();
                    ColibriNetworkManager colibriNetworkManager = FindObjectOfType<ColibriNetworkManager>();
                    if (menuManager.activePlayer == colibriNetworkManager.self.Id)
                    {
                        //be ready to send parent transform data (localposition!) to other players once they arrived
                        Sync.Receive("RequestParentTransform", (bool send) => {
                            Sync.Send("AnswerParentTransform", new JObject
                            {
                                { "localposX", parent.transform.localPosition.x },
                                { "localposY", parent.transform.localPosition.y },
                                { "localposZ", parent.transform.localPosition.z },
                                { "localrotX", parent.transform.localRotation.x },
                                { "localrotY", parent.transform.localRotation.y },
                                { "localrotZ", parent.transform.localRotation.z },
                                { "localrotW", parent.transform.localRotation.w },
                                { "localscaleX", parent.transform.localScale.x },
                                { "localscaleY", parent.transform.localScale.y },
                                { "localscaleZ", parent.transform.localScale.z }
                            });
                        });
                    }
                    else
                    {
                        //if not active player, query active player to send parent transform data
                        //on receive parent transform data, set parent transform to received data
                        Sync.Receive("AnswerParentTransform", (JToken obj) => {
                            parent.transform.localPosition = new Vector3(obj["localposX"].Value<float>(), obj["localposY"].Value<float>(), obj["localposZ"].Value<float>());
                            parent.transform.localRotation = new Quaternion(obj["localrotX"].Value<float>(), obj["localrotY"].Value<float>(), obj["localrotZ"].Value<float>(), obj["localrotW"].Value<float>());
                            parent.transform.localScale = new Vector3(obj["localscaleX"].Value<float>(), obj["localscaleY"].Value<float>(), obj["localscaleZ"].Value<float>());
                            });
                        Sync.Send("RequestParentTransform", true);

                    }
                    animation.transform.SetParent(parent.transform, true);

                    break;
                case AncorChange.TrackedToWorld:
                    //sceneObjects.transform.parent = this.transform;
                    animation.transform.parent = this.transform;
                    break;
                case AncorChange.TrackedToTracked:
                    break;
                case AncorChange.None:
                    break;
                default:
                    break;
            }

            // Decrement the counter when the animation is complete
            animationsRunning--;
        }
    }
}
