using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Mut
{
    // CONTROLLER FUNCTIONS
    public static Controller Meiosis(Controller organismOne, Controller organismTwo, Population population)
    {
        List<Controller> parents = new List<Controller>(){organismOne, organismTwo};
        Dictionary<int, Connection> recombinantConnectionGenome = new Dictionary<int, Connection>(){};
        List<int> connectionKeys = organismOne.connectionGenome.Keys.ToList();
        for (int i = 0; i < organismOne.connectionGenome.Count; i++)
        {
            if (organismOne.connectionGenome.ContainsKey(connectionKeys[i]) && organismTwo.connectionGenome.ContainsKey(connectionKeys[i])) {
                Controller passedParent = parents[UnityEngine.Random.Range(0,2)];
                recombinantConnectionGenome.Add(connectionKeys[i], passedParent.connectionGenome[connectionKeys[i]]);
                continue;
            }
            recombinantConnectionGenome.Add(connectionKeys[i], organismOne.connectionGenome[connectionKeys[i]]);
        }

        Dictionary<int, Node> recombinantNodeGenome = new Dictionary<int, Node>(){};
        List<int> nodeKeys = organismOne.nodeGenome.Keys.ToList();
        for (int i = 0; i < organismOne.nodeGenome.Count; i++)
        {
            if (organismOne.nodeGenome.ContainsKey(nodeKeys[i]) && organismTwo.nodeGenome.ContainsKey(nodeKeys[i])) {
                Controller passedParent = parents[UnityEngine.Random.Range(0,2)];
                recombinantNodeGenome.Add(nodeKeys[i], passedParent.nodeGenome[nodeKeys[i]]);
                continue;
            }
            recombinantNodeGenome.Add(nodeKeys[i], organismOne.nodeGenome[nodeKeys[i]]);
        }

        Controller offspring = new Controller(organismOne, recombinantNodeGenome, recombinantConnectionGenome, population);

        return offspring;
    }
    public static void MutateController(Controller controller, Dictionary<string, float> mods, Population population)
    {
        float fitnessMutMod = 1 - (controller.fitness/100);
        if (UnityEngine.Random.Range(0, 101) < (mods["mutationStrength"]*mods["nodeMod"]*fitnessMutMod*100)) { 
            Mut.CreateNode(controller, population);
        }
        if (UnityEngine.Random.Range(0, 101) < (mods["mutationStrength"]*mods["connectionMod"]*fitnessMutMod*100)) {
            Mut.CreateConnection(controller, population);
        }
        for (int i = 0; i < (controller.connections.Count); i++)
        {
            if (UnityEngine.Random.Range(0, 101) < (mods["mutationStrength"]*mods["enabledMod"]*fitnessMutMod*100)) {
                Mut.MutateEnabled(controller.connections[i]);
            }
            Mut.MutateWeight(controller.connections[i], mods["mutationStrength"]*mods["weightMod"]*fitnessMutMod); 
        }
        for (int i = 0; i < (controller.layers.Count); i++)
        { 
            for (int j = 0; j < (controller.layers[i].nodes.Count); j++)
            { 
                if (UnityEngine.Random.Range(0, 101) < (mods["mutationStrength"]*mods["activationFunctionMod"]*fitnessMutMod*100)) { 
                    Mut.MutateActivationFunction(controller.layers[i].nodes[j]); 
                }
            }
        }
    }
    // STRUCTURE FUNCTIONS
    public static void CreateConnection(Controller controller, Population population)
    {
        bool connectionFound = false; 
        int x = 0; 
        while (!connectionFound) 
        {
            connectionFound = true; // Assumption made to make code extractable further down

            Layer firstLayer = controller.layers[UnityEngine.Random.Range(0, (controller.layers.Count - 1))];
            Node firstNode = firstLayer.nodes[UnityEngine.Random.Range(0, firstLayer.nodes.Count)];

            Layer secondLayer = controller.layers[UnityEngine.Random.Range(controller.layers.IndexOf(firstLayer) + 1, controller.layers.Count)];
            Node secondNode = secondLayer.nodes[UnityEngine.Random.Range(0, secondLayer.nodes.Count)];

            for (int i = 0; i < (firstNode.connections.Count); i++) 
            {
                if (firstNode.connections[i].outNode == secondNode) { 
                    // Invalid as the proposed connection would be a double up
                    connectionFound = false; 
                }
            }
            x++;
            if (x > 50) { 
                return;
            }
            if (connectionFound) { // This code must be run here because the firstNode and secondNode don't exist outside the while loop
                Connection connection = new Connection(firstNode, secondNode, controller, population.connectionIndex); 
            }
        }
    }
    public static void CreateNode(Controller controller, Population population)
    {
        Connection oldConnection = controller.connections[UnityEngine.Random.Range(0, controller.connections.Count)];
        Node inputNode = oldConnection.inNode; 
        Node outputNode = oldConnection.outNode;
        Node newNode;

        if ((controller.layers.IndexOf(outputNode.layer) - controller.layers.IndexOf(inputNode.layer)) == 1) { // If there isn't a layer between the two nodes
            Layer newLayer = new Layer(1, (controller.layers.Count), controller, population.nodeIndex); 
            controller.layers.Insert((controller.layers.IndexOf(inputNode.layer) + 1), newLayer);
            newNode = newLayer.nodes[0];
        } else { // If there are layers between the input and output
            newNode = new Node(controller.layers[(controller.layers.IndexOf(inputNode.layer) + 1)], controller, population.nodeIndex);
        }

        Connection firstConnection = new Connection(inputNode, newNode, controller, population.connectionIndex);
        Connection secondConnection = new Connection(newNode, outputNode, controller, population.connectionIndex);
        oldConnection.connectionEnabled = false;
    }

    // CONNECTION FUNCTIONS
    public static void MutateWeight(Connection connection, float mutationStrength)
    {
        float weightModifier = UnityEngine.Random.Range(0.1f, connection.weight*mutationStrength);
        int posNeg = UnityEngine.Random.Range(0, 2)*2 - 1;
        connection.weight += weightModifier*posNeg;
        if (Math.Abs(connection.weight) > 2) {
            connection.weight = 2*Math.Sign(connection.weight);
        }
    }
    public static void MutateEnabled(Connection connection)
    {
        connection.connectionEnabled = !connection.connectionEnabled; 
    }

    // NODE FUNCTIONS
    public static void MutateActivationFunction(Node node)
    {
        // List of activation functions
        List<Node.ActivationFunction> activationFunctions = new List<Node.ActivationFunction>(){Node.ReLU, Node.Sigmoid, Node.Tanh};
        activationFunctions.Remove(node.activationFunction);
        int length = activationFunctions.Count;
        node.activationFunction = activationFunctions[UnityEngine.Random.Range(0, length)];
    }
}