using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using HCIKonstanz.Colibri.Synchronization;


namespace com.animationauthoring
{
    public class MenuScript : MonoBehaviour
    {
        public GameObject prefab;
        public MenuManager menuManager;

        private void Start()
        {
            GetComponent<PressableButton>().ButtonPressed.AddListener(() => CheckNetworkLoadPrefab());
        }



        private void CheckNetworkLoadPrefab()
        {
            //do it once for the network and once for local
            menuManager.LoadPrefab(prefab.name);
            Sync.Send("LoadPrefab", (string) prefab.name);
        }

    }
    }