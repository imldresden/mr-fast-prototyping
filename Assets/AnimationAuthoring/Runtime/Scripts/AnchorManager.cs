using com.animationauthoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform LeftHandAnchor;
    public Transform RightHandAnchor;
    public Transform LeftSideBodyAnchor;
    public Transform BodyAnchor;
    public static AnchorManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }



    public Transform AnchorToTransform(Ancor ancor)
    {
        switch (ancor)
        {
            case Ancor.Left_Hand:
                return LeftHandAnchor;
                
            case Ancor.Right_Hand:
                return RightHandAnchor;
                
            case Ancor.World:
                return null;
                
            case Ancor.LeftSideBody:
                return LeftSideBodyAnchor;
                
            case Ancor.Body:
                return BodyAnchor;
                
            default:
                return null;
               
        }

    }

    //get all ancors as a list
    public List<Transform> GetAllAncors()
    {
        List<Transform> allAncors = new List<Transform>();
        if (LeftHandAnchor != null)
        {
            allAncors.Add(LeftHandAnchor);
        }
        if (RightHandAnchor != null)
        {
            allAncors.Add(RightHandAnchor);
        }
        if (LeftSideBodyAnchor != null)
        {
            allAncors.Add(LeftSideBodyAnchor);
        }
        if (BodyAnchor != null)
        {
            allAncors.Add(BodyAnchor);
        }
        return allAncors;
    }
}