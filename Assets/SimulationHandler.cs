using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class codeTesting : MonoBehaviour
{
    [SerializeField] GameObject bonePrefab;

    int runGenerations = -1;
    Population testPop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        testPop = new Population(100, 3, 1, 0.25f, 7, this.bonePrefab);
    }

    public void Reset()
    {
        testPop = new Population(100, 3, 1, 0.25f, 7, this.bonePrefab);
    }

    public void StartSimulation(int desiredGenerations)
    {
        runGenerations = desiredGenerations;
    }

    public void RunSimulation()
    {
        testPop.Mutate();
        testPop.EvaluatePopulation();
        testPop.Reproduce();
    }

    void Update()
    {
        testPop.zLock();
        if (this.runGenerations > 0) {
            runGenerations--;
            if (this.testPop.maximumFitnesses.Count != 0 && this.testPop.maximumFitnesses[this.testPop.maximumFitnesses.Count - 1] > 99.99f) {
                runGenerations = 0;
            } else {
                this.RunSimulation();
            }
        }
    }
}
