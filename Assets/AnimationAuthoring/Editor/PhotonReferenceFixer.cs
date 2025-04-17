using UnityEngine;
using UnityEditor;
/*
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;

namespace com.animationauthoring
{
    [InitializeOnLoad]
    public class PhotonReferenceFixer
    {
        static PhotonReferenceFixer()
        {
            // This will run as soon as the scripts are recompiled
            EditorApplication.delayCall += FixPhotonReferences;
        }

        static void FixPhotonReferences()
        {
            // Find all GameObjects in the scene
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                // Get the PhotonView component
                PhotonView photonView = go.GetComponent<PhotonView>();
                if (photonView != null)
                {
                    // Get the PhotonTransformView component
                    PhotonTransformView photonTransformView = go.GetComponent<PhotonTransformView>();
                    if (photonTransformView != null)
                    {
                        // Check if the PhotonTransformView is already in the ObservedComponents list
                        if (!photonView.ObservedComponents.Contains(photonTransformView))
                        {
                            // Add the PhotonTransformView to the ObservedComponents list
                            photonView.ObservedComponents.Add(photonTransformView);
                            Debug.Log("Lonely PhotonTransformView found! Added PhotonTransformView to ObservedComponents list on " + go.name);
                        }
                    }
                }
            }
        }
    }
}
#endif*/