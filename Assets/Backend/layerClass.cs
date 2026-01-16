using UnityEngine;
using System.Collections.Generic;

public class Layer
{
    public int layerNumber; // Chronoligcal ID of the layer, NOT TOPOLOGICAL
    public List<Node> nodes = new List<Node>{}; 

    public Layer(int numNodes, int layerNumber, Controller controller, Dictionary<string, int> innovationIndex)
    {
        this.layerNumber = layerNumber;
        for (int i = 0; i < numNodes; i++)
        {
            new Node(this, controller, innovationIndex);
        }
    }
}