using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTracking : MonoBehaviour
{
    public void SwapSolvers()
    {
        this.gameObject.GetComponent<SolverHandler>().UpdateSolvers = !this.gameObject.GetComponent<SolverHandler>().UpdateSolvers;
    }
}
