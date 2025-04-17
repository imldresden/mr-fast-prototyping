using com.animationauthoring;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.animationauthoring
{
    public class CopyMixedRealityTransform : MonoBehaviour
    {
        public Ancor ancor;
        private Camera mainCamera;
        private MenuManager menuManager;
        private ColibriNetworkManager colibriNetworkManager;

        void Start()
        {
            mainCamera = Camera.main;
            menuManager = FindObjectOfType<MenuManager>();
            colibriNetworkManager = FindObjectOfType<ColibriNetworkManager>();
            if (menuManager == null)
            {
                Debug.LogError("No MenuManager found in scene");
            }
            if (colibriNetworkManager == null)
            {
                Debug.LogError("No ColibriNetworkManager found in scene");
            }
        }

        void Update()
        {
            if(colibriNetworkManager.self.Id == menuManager.activePlayer)
            {
                switch (ancor)
                {
                    case Ancor.Left_Hand:
                        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out var palm))
                        {
                            transform.position = palm.Position;
                            //transform.rotation = palm.Rotation; // Copy rotation
                        }
                        break;
                    case Ancor.Right_Hand:
                        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palm))
                        {
                            transform.position = palm.Position;
                            //transform.rotation = palm.Rotation; // Copy rotation
                        }
                        break;
                    case Ancor.Body:
                        transform.position = mainCamera.transform.position;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
