using com.animationauthoring;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

namespace com.animationauthoring { 
public class MakeActorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            /*
#if PHOTON_UNITY_NETWORKING

        GetComponent<PressableButton>().ButtonPressed.AddListener(() => MakePlayerActor());
#endif*/
            this.enabled = false;
    }
        /*
    private void MakePlayerActor() {
#if PHOTON_UNITY_NETWORKING
        GameObject.Find("GameManager").GetComponent<GameManager>().setSpectator(false);
            Debug.Log("Made this User an Actor");
#endif
    }*/
}
}
