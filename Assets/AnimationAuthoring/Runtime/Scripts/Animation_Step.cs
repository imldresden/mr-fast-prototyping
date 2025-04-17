using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;
namespace com.animationauthoring
{
    /// <summary>
    /// Class to hold parameters for each Animation Step. Each object with Animation Step script is animated in the order it has in the unity inspector.
    /// </summary>
    public class Animation_Step : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The trigger that starts the animation step.")]
        public String trigger;

        [SerializeField]
        [Tooltip("The method used to animate the animation steps")]
        public AnimationStyle animationStyle;

        [SerializeField]
        [Tooltip("The length of the duration in seconds")]
        [Range(0f,5f)]
        public float animationDuration = 1f;

        [SerializeField]
        [Tooltip("Whether the step should be animated or not")]
        public bool doAnimate = true;

        public Animation_Step(string trigger, AnimationStyle animationStyle, float animationDuration, bool doAnimate)
        {
            this.trigger = trigger;
            this.animationStyle = animationStyle;
            this.animationDuration = animationDuration;
            this.doAnimate = doAnimate;
        }
    }
}

