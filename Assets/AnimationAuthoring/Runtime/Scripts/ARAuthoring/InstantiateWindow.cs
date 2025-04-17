using com.animationauthoring;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstantiateWindow : MonoBehaviour
{

    public GameObject windowPrefab;
    [HideInInspector]
    public GameObject newWindow;
    public void Start()
    {
    }
    public void InstantiateWindowPrefab()
    {
        if (windowPrefab == null)
        {
            Debug.LogError("windowPrefab is not set in the InstantiateWindow script");
            return;
        }
        if (newWindow != null)
        {
            GameObject.Destroy(newWindow);
        }
        newWindow = Instantiate(windowPrefab);

            //Fetch the current scene and instantiate the new windows
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null)
            {
                // MenuManager gefunden, hier können Sie Aktionen mit menuManager ausführen
            }
            else
            {
                Debug.LogError("Could not find a MenuManager Script");
            }
            GameObject sequenceObject = menuManager.GetCurrentScene();
            if (sequenceObject != null)
            {
                MenuController[] controllers = newWindow.GetComponentsInChildren<MenuController>();
                foreach (MenuController controller in controllers)
                {
                    controller.SetSequenceObject(sequenceObject);
                }
            }
            //Debug.Log("Instantiated new window");
    }


    public void ToggleWindow()
    {
        if (newWindow == null)
        {
            Debug.LogError("No Window has been initialized");
            return;
        }
        newWindow.SetActive(!newWindow.activeSelf);
        if (newWindow.activeSelf)
        {
           newWindow.GetComponent<SolverHandler>().UpdateSolvers = true;
        }
        else
        {
            newWindow.GetComponent<SolverHandler>().UpdateSolvers = false;
        }
        //Debug.Log("Toggled window");
    }
}
