using HCIKonstanz.Colibri.Synchronization;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using TMPro;



namespace com.animationauthoring
{
    public class MenuManager : MonoBehaviour
    {
        [HideInInspector]
        public GameObject currentScene;
        public GameObject buttonPrefab; // Prefab for the button
        //Position the Prefabs will be intialized at
        [SerializeField]
        public GameObject prefabPosition;

        public List<GameObject> prefabs; // List of prefabs
        public int activePlayer = 1;
        private GameObject newParent; 

        private void Start()
        {
            Instantiate();
            SetupListeners();
        }
        void SetupListeners()
        {
            Sync.Receive("LoadPrefab", (string prefabName) =>
            {
                //Debug.Log(prefabName);
                //For some reason the prefab name is in quotes, we need to remove those.
                prefabName = prefabName.Trim('"');
                LoadPrefab(prefabName);
            });
            Sync.Receive("SetActivePlayer", (int activePlayer) =>
            {
                SetActivePlayer(activePlayer);
            });
        }

        public void Instantiate()
        {
            // Create a button for each prefab
            for (int i = 0; i < prefabs.Count; i++)
            {
                // Instantiate the button prefab
                GameObject button = Instantiate(buttonPrefab, transform);
                GameObject actorButton = this.transform.Find("Actor-Button").gameObject;
                Vector3 newPosition = actorButton.transform.position - new Vector3(0, 0.125f * i, 0.15f);
                button.transform.position = newPosition;
                button.transform.rotation = actorButton.transform.rotation;

                // Set the button label
                button.GetComponentInChildren<TextMeshPro>().text = prefabs[i].name;
                // Add the MenuScript to the button and set the MenuManager and prefab
                MenuScript menuScript = button.AddComponent<MenuScript>();
                menuScript.menuManager = this;
                menuScript.prefab = prefabs[i];

            }
        }


        public void LoadPrefab(string prefabName)
        {
            GameObject prefab = prefabs.Find(p => p.name == prefabName);
            if (prefab != null)
            {
                StartCoroutine(LoadPrefabCoroutine(prefab));
            }
            else
            {
                Debug.LogError("Prefab not found: " + prefabName);
            }
        }

        private IEnumerator LoadPrefabCoroutine(GameObject prefab)
        {
            if (this.DoesSceneExist())
            {

                //Sync.Send("DestroySequence", true);
                currentScene.GetComponent<Sequence>().DestroySequence(true);
                if (newParent != null)
                {
                    GameObject.Destroy(newParent);
                }
            }
            yield return new WaitForSeconds(0.5f);
            //add photon network components to the prefab
            
            if (prefab.GetComponent<SyncTransform>() == null)
            {
                prefab.AddComponent<SyncTransform>();
            }
            currentScene = Instantiate(prefab);
            //photonView.RPC("SetTransform", RpcTarget.All, currentScene.GetComponent<PhotonView>().ViewID);

            //create a new parent object, parent the current scene and the parent the new parent 
            CreateParentAndSetTransforms();

        }
        /* Redundant with Colibri
        public void SetTransform(int viewID)
        {
            PhotonView photonView = PhotonView.Find(viewID);
            if (photonView != null)
            {
                currentScene = photonView.gameObject;
            }
            else
            {
                Debug.LogError("No GameObject (currentscene) with PhotonView ID " + viewID + " found.");
            }
        CreateParentAndSetTransforms();
        }*/

        public void CreateParentAndSetTransforms()
        {
            newParent = new GameObject("Parent");
            newParent.transform.SetParent(prefabPosition.transform, true);
            currentScene.transform.SetParent(newParent.transform, true);
            newParent.transform.position = prefabPosition.transform.position;
            newParent.transform.rotation = prefabPosition.transform.rotation;
            MenuController[] menuManager = FindObjectsOfType<MenuController>();
            foreach (MenuController controller in menuManager)
            {
                controller.SetSequenceObject(currentScene);
            }

        }

        /// <summary>
        /// Set the active player that will have the objects tracked to their hands
        /// </summary>
        /// <param name="playernumber"> The number of the active player that starts the action to track the objects</param>
        public void SetActivePlayer(int playernumber)
        {
            //Debug.Log("Set Active Player: " + playernumber);
            activePlayer = playernumber;
        }
        //This method is likely redundant with Colibri
        /*
        public void TransferOwnership(int actorNumber)
        {
            List<Transform> allAnchors = AnchorManager.Instance.GetAllAncors();

            foreach (Transform anchor in allAnchors)
            {
                anchor.GetComponent<SyncTransform>();
            }
        }
        */
        public GameObject GetCurrentScene()
        {
            return currentScene;
        }

        public bool DoesSceneExist()
        {
            return currentScene != null;
        }
    }
}
