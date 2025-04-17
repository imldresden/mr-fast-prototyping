using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.animationauthoring
{
    public class Trigger : MonoBehaviour, ITriggers
    {

    public List<string> trigger { get; private set; } = new List<string>
        {
            "None",
            "Spacebar",
            "Something new"
        };

            /// <summary>
            /// Function that evaluates the trigger-enum of a given animation_step and calls the respective function.
            /// </summary>
            /// <param name="animation_Step">Animation_step object, which trigger parameter should be evaluated and called</param>
            /// <returns>true when the specific trigger function associated with the trigger evaluates to true</returns>
            public bool EvalAndCallTrigger(string animation_Step)
            {
                switch (animation_Step)
                {
                    case "None":
                        // Handle None case if needed
                        return true; // Return true or false based on your logic

                    case "Spacebar":
                        return SpacebarTrigger();

                    case "Something new":
                        Debug.LogError("This is a showcase trigger, implement your own triggers like this!");

                        return true; // Return true or false based on your logic
                    // Add more cases for other triggers as needed

                    default:
                        Debug.LogWarning("Invalid Trigger given");
                        return true; // Default behavior, you can adjust this as needed
                }

            }
            /// <summary>
            /// Function evaluates to true when the spacebar is pressed
            /// </summary>
            /// <returns>true when spacebar is pressed down</returns>
            public bool SpacebarTrigger()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    return true;
                }
                return false;
            }


            public List<string> GetTrigger()
            {
                return trigger;
            }
        }

}
