//using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

namespace com.animationauthoring
{

    /// <summary>
    /// Class that acts as an enum for our use-cases
    /// </summary>
    public interface ITriggers
    {
        List<string> trigger { get;}
        bool EvalAndCallTrigger(string animation_Step);
    }
}