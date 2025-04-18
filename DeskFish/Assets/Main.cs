using System;
using UnityEngine;

public class Main : MonoBehaviour
{
    [NonSerialized] public Skeleton skeleton;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.skeleton = new Skeleton(7);
    }

    // Update is called once per frame
    void Update()
    {
        this.skeleton.UpdateSkeleton();
    }
}
