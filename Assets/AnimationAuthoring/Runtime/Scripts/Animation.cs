using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.animationauthoring
{
    /// <summary>
    /// Class that handles animation of parent object for a given transform. Animates position, rotation and scale over duration
    /// </summary>
    public class Animation : MonoBehaviour
    {
        /// <summary>
        /// Function that starts animation Coroutine with given parameters.
        /// </summary>
        /// <param name="targetTransform">New transform the object is animated to</param>
        /// <param name="animationDuration">The length of the animation</param>
        /// <param name="animationStyle">The style of the animation</param>
        public void MoveObject(Vector3 targetLocalPosition, Quaternion targetLocalRotation, Vector3 targetLocalScale, float animationDuration, AnimationStyle animationStyle)
        {
            StartCoroutine(AnimateToTarget(targetLocalPosition, targetLocalRotation, targetLocalScale, animationDuration, animationStyle));
        }
        /// <summary>
        /// Coroutine that animates object scale, position and rotation.
        /// </summary>
        /// <param name="targetTransform">New transform the object is animated to</param>
        /// <param name="animationDuration">The length of the animation</param>
        /// <param name="animationStyle">The style of the animation</param>
        /// <returns>When the animation is finished</returns>
        public IEnumerator AnimateToTarget(Vector3 targetLocalPosition, Quaternion targetLocalRotation, Vector3 targetLocalScale, float animationDuration, AnimationStyle animationStyle)
        {
            Vector3 lastPosition = transform.localPosition;
            Quaternion lastRotation = transform.localRotation;
            Vector3 lastScale = transform.localScale;

            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                float t = SmoothTransition(elapsedTime / animationDuration, animationStyle);

                transform.localPosition = Vector3.Lerp(lastPosition, targetLocalPosition, t);
                transform.localRotation = Quaternion.Slerp(lastRotation, targetLocalRotation, t);
                transform.localScale = Vector3.Lerp(lastScale, targetLocalScale, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = targetLocalPosition;
            transform.localRotation = targetLocalRotation;
            transform.localScale = targetLocalScale;
            yield return new WaitForSeconds(animationDuration);
        }

        private float SmoothTransition(float t, AnimationStyle animationStyle)
        {
            // Custom easing function for smooth transitions
            if (animationStyle == AnimationStyle.InterpolateToLinear)
            {
                // Transition from cubic interpolation to linear interpolation
                return t * t;
            }
            else if (animationStyle == AnimationStyle.LinearToInterpolate)
            {
                // Transition from linear interpolation to cubic interpolation
                return 1f - (1f - t) * (1f - t);
            }
            else if (animationStyle == AnimationStyle.Linear)
            {
                return t;
            } else
            {
                // Use cubic interpolation for other styles
                return t * t * (3f - 2f * t);
            }
        }

        /// <summary>
        /// Method to check whether object has reached new position, to check whether the animation is complete
        /// </summary>
        /// <param name="targetTransform">The transform that holds the position that is used for the check</param>
        /// <returns>true when the target transforms position is reached</returns>
        public bool IsAnimationComplete(Vector3 targetLocalPosition)
        {
            return targetLocalPosition == transform.localPosition;
        }

    }
}
