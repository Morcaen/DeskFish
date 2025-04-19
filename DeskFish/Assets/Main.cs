using System;
using UnityEngine;

public class Main : MonoBehaviour
{
    [NonSerialized] public Skeleton skeleton;
    [SerializeField] Visualiser visualiser;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.skeleton = new Skeleton(7);
        this.visualiser.InitialiseSkeletonVisualisation(this.skeleton);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            this.skeleton.UpdateSkeleton();
        }
        this.visualiser.UpdateSkeletonVisualisation();
        this.skeleton.MoveSkeleton(new Vector3(0, 1*Time.deltaTime, 0));
    }
}
