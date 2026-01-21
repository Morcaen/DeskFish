using UnityEngine;
using System.Collections.Generic;
public class Connection
{
    public Node inNode;
    public Node outNode;
    public float weight = Random.Range(0f, 1f);
    public bool connectionEnabled = true;
    public int innovationNumber;

    public Connection(Node inNode, Node outNode, Controller controller, Dictionary<string, int> innovationIndex)
    {
        string nodePair = $"[{inNode.nodeNumber}, {outNode.nodeNumber}]";
        if (innovationIndex.ContainsKey(nodePair)) {
            this.innovationNumber = innovationIndex[nodePair];
        } else {
            this.innovationNumber = innovationIndex.Count;
            // This is done before adding so that the innovation numbers start counting from zero, which would be the index of the first entry, rather than from 1
            innovationIndex.Add(nodePair, this.innovationNumber);
        }
        this.inNode = inNode;
        this.inNode.connections.Add(this);
        this.outNode = outNode;
        controller.connectionGenome.Add(this.innovationNumber, this);
        controller.connections.Add(this);
    }
    public void Calculate()
    {
        if (this.connectionEnabled) {
            this.outNode.rawValue += this.inNode.activationValue*this.weight;
        }
    }
}
