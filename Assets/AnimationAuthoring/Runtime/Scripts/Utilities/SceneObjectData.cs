using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.animationauthoring
{

    public class SceneObjectData : MonoBehaviour
    {
        public GameObject gameObj;
        public Ancor ancor;
        public Vector3 lastWorldPosition;
        public int id; 

        public SceneObjectData(GameObject gameObject, Ancor ancor, int id)
        {
            this.gameObj = gameObject;
            this.ancor = ancor;
            this.id = id; 
        }
    }
}
