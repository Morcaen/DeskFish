using UnityEngine;
using System;
using System.Collections.Generic;
public class Node
{
    public Layer layer;
    public int nodeNumber;
    public double activationValue = 0d;
    public double rawValue = 0d;
    public bool isInputNode = false;
    public bool isBiasNode = false;
    public List<Connection> connections = new List<Connection>(){};
    
    // Initialisation Function
    public Node(Layer layer, Controller controller, Dictionary<string, int> innovationIndex)
    {  
        this.layer = layer; 
        this.layer.nodes.Add(this); 
        string nodeID = $"[{this.layer.layerNumber}, {this.layer.nodes.Count}]";
        if (innovationIndex.ContainsKey(nodeID)) { 
            this.nodeNumber = innovationIndex[nodeID]; 
        } else { 
            this.nodeNumber = innovationIndex.Count; 
            // This is done before adding the node to the index so that counting begins from zero
            innovationIndex.Add(nodeID, this.nodeNumber); 
        }
        controller.nodeGenome.Add(this.nodeNumber, this);
    }

    // Activation Code
    public static double ReLU(double value) // ReLU returns 0 for x <= 0, and returns x for x > 0 
    {
        return (value + Math.Abs(value))/2; // https://en.wikipedia.org/wiki/Rectifier_(neural_networks)
    }
    public static double Sigmoid(double value) // 'Squeezes' x values so they lie between -1 and 1
    {
        return 1/(1+Math.Exp(-value)); // https://en.wikipedia.org/wiki/Sigmoid_function 
    }
    public static double Tanh(double value) // Similar to Sigmoid
    {
        return (Math.Exp(value) - Math.Exp(-value))/(Math.Exp(value) + Math.Exp(-value)); // https Bames Jartlett
    }

    public delegate double ActivationFunction(double value);
    public ActivationFunction activationFunction = Sigmoid;
    
    public void CalculateActivation()
    {
        if (this.isInputNode) {
            return;
        } else if (this.isBiasNode) {
            this.activationValue = 1d;
            return;
        }
        this.activationValue = this.activationFunction(this.rawValue);
    }
}