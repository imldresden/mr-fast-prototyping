using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.animationauthoring
{
    public class AnimationChild : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 oldPosition = Vector3.zero;
        [HideInInspector]
        public Quaternion rotationOffset = Quaternion.identity;
        [SerializeField]
        [Tooltip("The place the animation is anchored to")]
        public Ancor ancor = Ancor.World;

        [SerializeField]
        [Tooltip("The offset that is applied to the ancor.")]
        public Vector3 ancorOffset;
        [SerializeField]
        [Tooltip("The Object ID used to identify object throught different Animation Steps")]
        public int objectID;
    }
}
