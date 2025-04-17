using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuManager : MonoBehaviour, IMixedRealityHandJointHandler
{
    public GameObject propertyMenuPrefab; // The prefab for the property menu
    public GameObject objectMenuPrefab; // The prefab for the object menu
    public GameObject animationStepMenuPrefab; // The prefab for the animation step menu

    private GameObject propertyMenuInstance;
    private GameObject objectMenuInstance;
    private GameObject animationStepMenuInstance;

    private bool isMenuVisible = false;


    void Start()
    {
        // Instantiate the menus
        propertyMenuInstance = Instantiate(propertyMenuPrefab);
        objectMenuInstance = Instantiate(objectMenuPrefab);
        animationStepMenuInstance = Instantiate(animationStepMenuPrefab);

        // Set the parent for the menus
        propertyMenuInstance.transform.SetParent(gameObject.transform, false);
        objectMenuInstance.transform.SetParent(gameObject.transform, false);
        animationStepMenuInstance.transform.SetParent(gameObject.transform, false);

        // Hide the menus initially
        propertyMenuInstance.SetActive(false);
        objectMenuInstance.SetActive(false);
        animationStepMenuInstance.SetActive(false);

        // Register this manager to receive hand joint events
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);

        // Example of adding button click listeners for the Object menu
        var objectMenuButtons = objectMenuInstance.GetComponentsInChildren<UnityEngine.UI.Button>();
        if (objectMenuButtons.Length >= 2)
        {
            objectMenuButtons[0].onClick.AddListener(() => OnObjectMenuButton1Clicked());
            objectMenuButtons[1].onClick.AddListener(() => OnObjectMenuButton2Clicked());
        }

        // Example of adding button click listeners for the Animation Step menu
        var animationStepMenuButtons = animationStepMenuInstance.GetComponentsInChildren<UnityEngine.UI.Button>();
        if (animationStepMenuButtons.Length >= 2)
        {
            animationStepMenuButtons[0].onClick.AddListener(() => OnAnimationStepMenuButton1Clicked());
            animationStepMenuButtons[1].onClick.AddListener(() => OnAnimationStepMenuButton2Clicked());
        }
    }

    void OnDestroy()
    {
        // Unregister the handler when the object is destroyed
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        // Check if the palm is facing the user
        if (eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out MixedRealityPose palmPose))
        {
            // Determine if the palm is approximately facing the user
            bool isPalmFacingUser = Vector3.Dot(palmPose.Up, Camera.main.transform.forward) > 0;

            // Update the menu visibility based on the palm orientation
            if (isPalmFacingUser && !isMenuVisible)
            {
                SetAllInstancesActive(true);
                isMenuVisible = true;
            }
            else if (!isPalmFacingUser && isMenuVisible)
            {
                SetAllInstancesActive(false);
                isMenuVisible = false;
            }
        }
    }

    //function to set all instances active or inactive
    public void SetAllInstancesActive(bool active)
    {
        animationStepMenuInstance.SetActive(active);
        objectMenuInstance.SetActive(active);
        propertyMenuInstance.SetActive(active);
    }

        // Example button click handlers
        private void OnObjectMenuButton1Clicked()
        {
            Debug.Log("Object Menu Button 1 Clicked");
        }

        private void OnObjectMenuButton2Clicked()
        {
            Debug.Log("Object Menu Button 2 Clicked");
        }

        private void OnAnimationStepMenuButton1Clicked()
        {
            Debug.Log("Animation Step Menu Button 1 Clicked");
        }

        private void OnAnimationStepMenuButton2Clicked()
        {
            Debug.Log("Animation Step Menu Button 2 Clicked");
        }
    }


