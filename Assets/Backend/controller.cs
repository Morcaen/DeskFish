using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using System;
public class Controller
{
    public float fitness = 0f;
    public Dictionary<string, int> connectionInnovationIndex;
    public Dictionary<string, int> nodeInnovationIndex;

    public int finishFrame;

    // GENOTYPE
    public List<Layer> layers = new List<Layer>{};
    public List<Connection> connections = new List<Connection>{};

    // GENOME
    public Dictionary<int, Node> nodeGenome = new Dictionary<int, Node>(){};
    public Dictionary<int, Connection> connectionGenome = new Dictionary<int, Connection>(){};

    public Controller(int inputSize, int outputSize, Dictionary<string, int> connectionInnovationIndex, Dictionary<string, int> nodeInnovationIndex) 
    {   
        this.connectionInnovationIndex = connectionInnovationIndex;
        this.nodeInnovationIndex = nodeInnovationIndex;

        // Layers

        this.layers.Add(new Layer(inputSize, 0, this, nodeInnovationIndex));
        this.layers.Add(new Layer(outputSize, 1, this, nodeInnovationIndex));

        // Input Nodes 

        this.layers[0].nodes[0].isBiasNode = true;
        for (int i = 1; i < this.layers[0].nodes.Count; i++)
        { // Starts at i = 1 to skip the bias node
            this.layers[0].nodes[i].isInputNode = true;
        }

        // Connections

        for (int i = 0; i < outputSize; i++)
        {
            // Creates a connection between the bias node and the 'i'th output node
            this.connections.Add(new Connection(this.layers[0].nodes[0], this.layers[1].nodes[i], this, connectionInnovationIndex));
        }

    }
    
    public Controller(Controller template, Dictionary<int, Node> templateNodeGenome, Dictionary<int, Connection> templateConnectionGenome, Population population)
    {
        this.nodeInnovationIndex = population.nodeIndex;
        this.connectionInnovationIndex = population.connectionIndex;
        for (int i = 0; i < template.layers.Count; i++)
        {
            this.layers.Add(new Layer((template.layers[i].nodes.Count), template.layers[i].layerNumber, this, population.nodeIndex));
            for (int j = 0; j < this.layers[i].nodes.Count; j++)
            {
                if (this.layers[i].nodes[j].nodeNumber != template.layers[i].nodes[j].nodeNumber) {
                    Debug.Log($"[{i}, {j}]: Node Numbers do not align");
                }
                int nodeNumber = this.layers[i].nodes[j].nodeNumber;
                this.layers[i].nodes[j].activationFunction = templateNodeGenome[nodeNumber].activationFunction;
            }
        }
        this.layers[0].nodes[0].isBiasNode = true;
        for (int i = 1; i < this.layers[0].nodes.Count; i++)
        {
            this.layers[0].nodes[i].isInputNode = true;
        }
        // This iteration must come separately from the previous one because all nodes must be created before connections can be initialised
        for (int i = 0; i < this.layers.Count; i++)
        {
            for (int j = 0; j < this.layers[i].nodes.Count; j++)
            {
                int nodeNumber = this.layers[i].nodes[j].nodeNumber;
                Node inputNode = this.layers[i].nodes[j];
                for (int k = 0; k < template.nodeGenome[nodeNumber].connections.Count; k++)
                {
                    int outputNodeNumber = template.nodeGenome[nodeNumber].connections[k].outNode.nodeNumber;
                    Node outputNode = this.nodeGenome[outputNodeNumber];
                    Connection newConnection = new Connection(inputNode, outputNode, this, this.connectionInnovationIndex);
                    newConnection.connectionEnabled = templateConnectionGenome[newConnection.innovationNumber].connectionEnabled;
                    newConnection.weight = templateConnectionGenome[newConnection.innovationNumber].weight;
                }
            }
        }
    }
    
    public void CalculateNetwork(List<float> inputs)
    {
        if (inputs.Count != (this.layers[0].nodes.Count - 1)) {
            Debug.Log($"Size of input ({inputs.Count}) != size of input layer ({this.layers[0].nodes.Count})");
            return;
        }
        for (int i = 1; i < this.layers[0].nodes.Count; i++)
        {
            this.layers[0].nodes[i].activationValue = inputs[i - 1]; 
        }
        this.CalculateNetwork();
    }

    public void CalculateNetwork()
    {
        for (int i = 0; i < this.layers.Count; i++)
        {
            for (int j = 0; j < this.layers[i].nodes.Count; j++) 
            {
                this.layers[i].nodes[j].CalculateActivation();
                this.layers[i].nodes[j].rawValue = 0;
                for (int k = 0; k < this.layers[i].nodes[j].connections.Count; k++)
                {
                    this.layers[i].nodes[j].connections[k].Calculate();
                }
            }
        }
    }

    public List<float> GetOutputs()
    {
        Layer outputLayer = this.layers[this.layers.Count - 1];
        List<float> outputs = new List<float>{};
        foreach (Node node in outputLayer.nodes)
        {
            outputs.Add((float) node.activationValue);
        }

        return outputs;
    }

    public void EvaluateFitness(float leadPos, bool didFinish)
    {
        // Write a fitness evaluation
        if (didFinish) {
            this.fitness = 6000 / this.finishFrame * 50 + 10;
            return;
        }
        float distance = leadPos + 18f;
        this.fitness = distance / 36 * 75;
        return;
    }
}