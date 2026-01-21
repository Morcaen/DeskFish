using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class codeTesting : MonoBehaviour
{
    [SerializeField] GameObject bonePrefab;
    [SerializeField] BoxCollider groundCollider;

    int runGenerations = 100;
    int runFrames = 6000;
    int frame = 0;
    Population testPop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testPop = new Population(100, 9, 3, 2f, 4, this.bonePrefab);
        testPop.Mutate();
    }

    public void Reset()
    {
        this.frame = 0;
        testPop = new Population(100, 9, 3, 1f, 4, this.bonePrefab);
        testPop.Mutate();
    }

    public void StartSimulation(int desiredGenerations)
    {
        runGenerations = desiredGenerations;
    }

    public void StartNextGeneration()
    {
        this.frame = 0;
        this.runFrames = 6000;
        testPop.EvaluatePopulation();
        testPop.Reproduce();
        this.runGenerations--;
    }

    public void RunSimulation()
    {
        testPop.Run(this.frame, groundCollider);
    }

    void Update()
    {
        this.frame++;
        testPop.zLock();
        if (this.runFrames > 0) {
            runFrames--;            
            this.RunSimulation();
        } else {
            if (this.runGenerations > 0) {
                this.StartNextGeneration();
            }
        }
    }
}