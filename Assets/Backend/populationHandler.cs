using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Population
{
    public Dictionary<string, float> mutationMods = new Dictionary<string, float>()
    {
        {"mutationStrength", 0.25f},
        {"weightMod", 1f},
        {"enabledMod", 0.01f},
        {"connectionMod", 0.001f},
        {"nodeMod", 0.001f},
        {"activationFunctionMod", 0.1f} 
    };
    float compatibilityThreshold = 1f;
    float reproductionAggresivity = 0.1f;
    int targetPopulationSize;
    public int generation = 0;
    int stagnantGenerations = 0;
    public List<Controller> brains = new List<Controller>(){};
    public List<Skeleton> skeletons = new List<Skeleton>(){};
    public Dictionary<string, int> connectionIndex = new Dictionary<string, int>(){};
    public Dictionary<string, int> nodeIndex = new Dictionary<string, int>(){};
    public Dictionary<int, Species> communityDictionary = new Dictionary<int, Species>(){};
    public List<Species> community = new List<Species>(){};
    public List<Species> extinctCommunity = new List<Species>(){};
    public List<float> maximumFitnesses = new List<float>(){};
    List<float> averageFitnesses = new List<float>(){};
    
    public Population(int populationSize, int inputSize, int outputSize, float mutationStrength, int numBones, GameObject bonePrefab)
    {
        this.mutationMods["mutationStrength"] = mutationStrength;
        this.targetPopulationSize = populationSize;

        for (int i = 0; i < populationSize; i++)
        { 
            this.brains.Add(new Controller(inputSize, outputSize, this.connectionIndex, this.nodeIndex));
            this.skeletons.Add(new Skeleton(numBones, bonePrefab, new Color((float) i/populationSize, (float) i/populationSize, (float) i/populationSize, 1)));
        }
    }

    public void Mutate()
    {
        for (int i = 0; i < this.brains.Count; i++)
        {
            Mut.MutateController(this.brains[i], this.mutationMods, this);
        }
    }
    public void EvaluatePopulation()
    {
        for (int i = 0; i < this.brains.Count; i++)
        {
            this.brains[i].EvaluateFitness();
        }
    }
    public void Reproduce()
    {
        // Speciate the population
        this.Speciate();
       
        float populationAverageAdjustedFitness = 0f;
        this.maximumFitnesses.Add(0f);
        for (int i = 0; i < this.community.Count; i++)
        {
            if (this.community[i].speciesMembers.Count == 0) {
                this.Extinction(this.community[i]);
                continue;
            }
            this.community[i].CalculateAverageAdjustedFitness();
            populationAverageAdjustedFitness += this.community[i].averageAdjustedFitness;
            this.community[i].speciesMembers = this.community[i].speciesMembers.OrderByDescending(x => x.fitness).ToList();
            if (this.community[i].speciesMembers[0].fitness > this.maximumFitnesses[this.maximumFitnesses.Count - 1]) {
                this.maximumFitnesses[this.maximumFitnesses.Count - 1] = this.community[i].speciesMembers[0].fitness;
            }
        }
        this.averageFitnesses.Add(populationAverageAdjustedFitness/this.community.Count);

        if (this.generation != 0) {
            if (this.maximumFitnesses[this.generation] > 99f) {
                stagnantGenerations = 0;
            }
            if (this.Within(this.maximumFitnesses[this.generation], 5f, this.maximumFitnesses[this.generation - 1])) {
                stagnantGenerations++;
            } else {
                stagnantGenerations = 0;
            }
        }

        if (stagnantGenerations >= 20) { // Pg 10. https://nn.cs.utexas.edu/downloads/papers/stanley.ec02.pdf
            stagnantGenerations = 0;
            List<Species> stagnantSpecies = new List<Species>(){};
            for (int i = 0; i < this.community.Count; i++)
            {
                if (this.community[i].age > 5) {
                    stagnantSpecies.Add(this.community[i]);
                }
            }
            List<Species> cullSpecies = stagnantSpecies.OrderByDescending(x => x.maximumFitnesses[x.maximumFitnesses.Count - 1]).ToList();
            for (int i = 2; i < cullSpecies.Count; i++)
            {
                this.Extinction(cullSpecies[i]);
            }
            populationAverageAdjustedFitness = 10;
        }
        
        List<Controller> offspring = new List<Controller>(){};
        for (int i = 0; i < this.community.Count; i++)
        {
            this.community[i].CalculateOffspringAllowance(this.targetPopulationSize, populationAverageAdjustedFitness);
            if (this.community[i].offspringAllowance < 10) {
                if (this.community[i].age > 10) {
                    int excessAllowance = this.community[i].offspringAllowance;
                    this.Extinction(this.community[i]);
                    if (this.community.Count == 0) {
                        break;
                    }
                    int distributedAllowance = excessAllowance/this.community.Count + 1; // 1 is added because integer division rounds down. 
                    for (int j = 0; j < this.community.Count; j++)
                    {
                        this.community[j].offspringAllowance += distributedAllowance;
                    }
                    continue;
                }
                this.community[i].offspringAllowance = 10;
            }
            this.community[i].age++;

            this.community[i].populationHistory.Add(this.community[i].speciesMembers.Count);
            this.community[i].maximumFitnesses.Add(this.community[i].speciesMembers[0].fitness);
            this.community[i].medianFitnesses.Add(this.community[i].speciesMembers[this.community[i].speciesMembers.Count/2].fitness);
            this.community[i].minimumFitnesses.Add(this.community[i].speciesMembers[this.community[i].speciesMembers.Count - 1].fitness);
            
            int numReproductiveOrganisms = (int)Math.Ceiling(this.community[i].speciesMembers.Count*this.reproductionAggresivity);
            if (numReproductiveOrganisms > this.community[i].speciesMembers.Count) {
                numReproductiveOrganisms = this.community[i].speciesMembers.Count;
            }
            for (int j = 0; j < this.community[i].offspringAllowance; j++)
            {
                // Ensuring indexParentTwo > indexParentOne removes the need for a fitness check in Mut.CrossOver() as Parent One is known to be more fit than Parent Two
                // This setup has a slight risk of reproducing with the same parent twice, this is an intentional choice in order to allow species with just a single member to reproduce
                int indexParentOne = UnityEngine.Random.Range(0, numReproductiveOrganisms - 1);
                int indexParentTwo = UnityEngine.Random.Range(indexParentOne, numReproductiveOrganisms);

                offspring.Add(Mut.Meiosis(this.community[i].speciesMembers[indexParentOne], this.community[i].speciesMembers[indexParentTwo], this));
            }
        }

        while (offspring.Count < this.targetPopulationSize)
        {
            int indexParentOne = UnityEngine.Random.Range(0, brains.Count);
            int indexParentTwo = UnityEngine.Random.Range(0, brains.Count);

            offspring.Add(Mut.Meiosis(this.brains[indexParentOne], this.brains[indexParentTwo], this));
        }

        while (offspring.Count > this.targetPopulationSize)
        {
            offspring.Remove(offspring[UnityEngine.Random.Range(0, offspring.Count)]);
        }
        
        this.brains = offspring;
        this.generation++;
    }

    public void Speciate()
    {
        for (int i = 0; i < this.community.Count; i++)
        {
            this.community[i].speciesMembers.Clear();
        }
        for (int i = 0; i < (this.brains.Count); i++)
        {
            bool speciated = false;
            for (int j = 0; j < (this.community.Count); j++)
            {
                if (this.community[j].CheckCompatibility(this.brains[i])) {
                    speciated = true;
                    break;
                }
            }
            if (!speciated) {
                this.community.Add(new Species(this.brains[i], this.compatibilityThreshold, (this.community.Count + this.extinctCommunity.Count), this.generation));
                this.communityDictionary.Add((this.community.Count + this.extinctCommunity.Count - 1), this.community[this.community.Count - 1]);
            }
        }
        int extantSpecies = 0;
        for (int i = 0; i < this.community.Count; i++)
        {
            if (this.community[i].speciesMembers.Count > 0) {
                extantSpecies++;
            }
        }
        Debug.Log($"{extantSpecies} extant species after speciation");
    }

    public void Extinction(Species species)
    {
        this.brains = this.brains.Except(species.speciesMembers).ToList();
        species.speciesMembers.Clear();
        this.community.Remove(species);
        this.extinctCommunity.Add(species);
    }

    bool Within(float query, float range, float benchmark)
    {
        if (query < benchmark + range && query > benchmark - range) {
            return true;
        }
        return false;
    }

    public void zLock()
    {
        foreach (Skeleton skeleton in this.skeletons)
        {
            skeleton.zLock();
        }
    }

}