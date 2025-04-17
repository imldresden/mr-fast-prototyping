using com.animationauthoring;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace com.animationauthoring
{

public class MenuController : MonoBehaviour
{
    private GameObject sequenceGameObject; // Zuweisen im Inspector
    public GameObject buttonPrefab; // Zuweisen im Inspector



        public void SetSequenceObject(GameObject newSequenceObject)
        {
            sequenceGameObject = newSequenceObject;
            UpdateMenu();
        }

    private void UpdateMenu()
    {
        // Löschen aller vorhandenen Buttons
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Erstellen eines Buttons für jede AnimationStep-Instanz
        foreach (Transform child in sequenceGameObject.transform)
        {
            if (child.GetComponent<Animation_Step>() != null)
            {
                    GameObject newButton = Instantiate(buttonPrefab, transform);
                    newButton.GetComponentInChildren<ButtonConfigHelper>().MainLabelText = child.name;
            }

        }
        //+ button
        GameObject newAnimationStepButton = Instantiate(buttonPrefab, transform);
        var iconSet = newAnimationStepButton.GetComponent<ButtonConfigHelper>().IconSet;

        this.GetComponent<GridObjectCollection>().UpdateCollection();

    }
}
}