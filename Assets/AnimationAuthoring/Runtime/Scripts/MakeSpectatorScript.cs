using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


namespace com.animationauthoring
{

        public class MakeSpectatorScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {/*
    #if PHOTON_UNITY_NETWORKING

            GetComponent<PressableButton>().ButtonPressed.AddListener(() => MakePlayerSpectator());
#endif*/
            this.enabled = false;
        }
        /*
        private void MakePlayerSpectator()
        {
    #if PHOTON_UNITY_NETWORKING
            GameObject.Find("GameManager").GetComponent<GameManager>().setSpectator(true);
            Debug.Log("Made this User a Spectator");
    #endif
        }*/
    }

}
