using System.Collections.Generic;
using UnityEngine;

public class Visualiser : MonoBehaviour
{
    [SerializeField] GameObject jointTemplate;

    List<GameObject> visualisedJoints = new List<GameObject>();
    Skeleton visualisedSkeleton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void InitialiseSkeletonVisualisation(Skeleton skeleton)
    {
        this.visualisedSkeleton = skeleton;
        foreach (GameObject joint in this.visualisedJoints)
        {
            Destroy(joint);
        }

        this.visualisedJoints = new List<GameObject>();

        for (int i = 0; i < this.visualisedSkeleton.joints.Count; i++)
        {
            this.visualisedJoints.Add(Instantiate(this.jointTemplate, this.visualisedSkeleton.joints[i].pos, Quaternion.identity));
        }
    }

    public void UpdateSkeletonVisualisation()
    {
        for (int i = 0; i < this.visualisedSkeleton.joints.Count; i++)
        {
            this.visualisedJoints[i].transform.position = this.visualisedSkeleton.joints[i].pos;
        }
    }
}
