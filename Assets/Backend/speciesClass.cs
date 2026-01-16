using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class Species
{
    public Controller holotype;
    public List<Controller> speciesMembers = new List<Controller>(){};
    public int ID;
    private float compatibilityThreshold;
    public int age = 0;
    public int divergenceGeneration;
    public float averageAdjustedFitness;
    public int offspringAllowance;
    public List<float> maximumFitnesses = new List<float>(){};
    public List<float> medianFitnesses = new List<float>(){};
    public List<float> minimumFitnesses = new List<float>(){};
    public List<int> populationHistory = new List<int>(){};
    

    public Species(Controller holotype, float compatibilityThreshold, int ID, int divergenceGeneration)
    {
        this.holotype = holotype;
        this.speciesMembers.Add(holotype);
        this.compatibilityThreshold = compatibilityThreshold;
        this.ID = ID;
        this.divergenceGeneration = divergenceGeneration;
    }

    public void CalculateOffspringAllowance(int targetPopulation, float populationAverageAdjustedFitness)
    {
        float offspringProportion = this.averageAdjustedFitness/populationAverageAdjustedFitness;
        this.offspringAllowance =  (int)Math.Ceiling(targetPopulation*offspringProportion);
    }
    public void CalculateAverageAdjustedFitness()
    {
        float cumulativeAdjustedFitness = 0;
        for (int i = 0; i < this.speciesMembers.Count; i++)
        {
            cumulativeAdjustedFitness += this.speciesMembers[i].fitness;
        }
        this.averageAdjustedFitness = cumulativeAdjustedFitness/this.speciesMembers.Count;
    }
    public void Reset()
    {
        this.speciesMembers.Clear();
    }
    public bool CheckCompatibility(Controller candidate)
    {
        if (candidate == this.holotype || CompatibilityDistance(this.holotype, candidate) < this.compatibilityThreshold) {
            this.speciesMembers.Add(candidate);
            return true;
        }
        return false;
    }
    public static float CompatibilityDistance(Controller organismOne, Controller organismTwo) // Calculates the compatability distance between two controllers
    {   // Page 13: https://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf
        Controller higherNodeIndex;
        Controller lowerNodeIndex;
        if (organismOne.nodeGenome.Keys.Max() > organismTwo.nodeGenome.Keys.Max()) {
            higherNodeIndex = organismOne;
            lowerNodeIndex = organismTwo;
        } else {
            higherNodeIndex = organismTwo;
            lowerNodeIndex = organismOne;
        }
        Controller higherConnectionIndex;
        Controller lowerConnectionIndex;
        if (organismOne.connectionGenome.Keys.Max() > organismTwo.connectionGenome.Keys.Max()) {
            higherConnectionIndex = organismOne;
            lowerConnectionIndex = organismTwo;
        } else {
            higherConnectionIndex = organismTwo;
            lowerConnectionIndex = organismOne;
        }
        
        // This code feels like it should be able to be refactored, but I don't know how.
        // The code for connection calculations is identical to the code for node calculations
        // However the function would need to be able to take two different kinds of Dict in order to
        // remove repeated code

        int excessGenes = 0;

        excessGenes += Species.ExcessConnectionGenes(higherConnectionIndex, lowerConnectionIndex);
        excessGenes += Species.ExcessNodeGenes(higherNodeIndex, lowerNodeIndex);
        
        int sharedGenes = 0;
        int disjointGenes = 0;
        float summedWeightDifferences = 0f;
        for (int i = 0; i < lowerConnectionIndex.connectionGenome.Keys.Max(); i++)
        { 
            if (lowerConnectionIndex.connectionGenome.ContainsKey(i) && higherConnectionIndex.connectionGenome.ContainsKey(i)) {
                sharedGenes++;
                summedWeightDifferences += Math.Abs(lowerConnectionIndex.connectionGenome[i].weight - higherConnectionIndex.connectionGenome[i].weight);
            } else if (lowerConnectionIndex.connectionGenome.ContainsKey(i) || higherConnectionIndex.connectionGenome.ContainsKey(i)) {
                disjointGenes++;
            }
        }
        for (int i = 0; i < lowerNodeIndex.nodeGenome.Keys.Max(); i++)
        { 
            if (lowerNodeIndex.nodeGenome.ContainsKey(i) && higherNodeIndex.nodeGenome.ContainsKey(i)) {
                sharedGenes++;
            } else if (lowerNodeIndex.connectionGenome.ContainsKey(i) || higherNodeIndex.nodeGenome.ContainsKey(i)) {
                disjointGenes++;
            }
        }
        
        float averageWeightDifference = 0f;
        if (sharedGenes > 0) {
            averageWeightDifference = summedWeightDifferences/sharedGenes;
        }

        float excessWeighting = 5f;
        float disjointWeighting = 2f;
        float sharedWeighting = 1f;
        // This line isn't actually right, higher maximum index does not mean larger genome 
        int numGenes = higherConnectionIndex.connectionGenome.Count + higherNodeIndex.nodeGenome.Count;
        float excessGeneDistance = (excessWeighting*excessGenes)/numGenes;
        float disjointGeneDistance = (disjointWeighting*disjointGenes)/numGenes;
        float weightDistance = (sharedWeighting*averageWeightDifference);
        float distance = excessGeneDistance + disjointGeneDistance + weightDistance;
        return Math.Abs(distance);
    }

    private static int ExcessConnectionGenes(Controller higherIndex, Controller lowerIndex)
    {
        int excessGenes = 0;
        for (int i = lowerIndex.connectionGenome.Keys.Max(); i < higherIndex.connectionGenome.Keys.Max(); i++)
        { 
            if (higherIndex.connectionGenome.ContainsKey(i)){ 
                excessGenes++; 
            }
        }
        return excessGenes;
    }

    private static int ExcessNodeGenes(Controller higherIndex, Controller lowerIndex)
    {
        int excessGenes = 0;
        for (int i = lowerIndex.nodeGenome.Keys.Max(); i < higherIndex.nodeGenome.Keys.Max(); i++)
        { 
            if (higherIndex.nodeGenome.ContainsKey(i)){ 
                excessGenes++;
            }
        }
        return excessGenes;   
    }
}
