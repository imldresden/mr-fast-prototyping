using UnityEngine;
using System.Collections.Generic;
/*
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;


public class PreparePool : MonoBehaviour
{
    public List<GameObject> Prefabs;

    void Start()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }
}
#endif*/
