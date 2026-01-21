using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Muscle> muscles = new List<Muscle> {};
    private List<GameObject> boneObjects = new List<GameObject> {};

    public bool finisher = false;

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numBones, GameObject bonePrefab, Color color)
    {
        // Make joints and bones
        this.boneObjects.Add(GameObject.Instantiate(bonePrefab, new Vector3(-18, 0, 0), new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 4), Mathf.Cos(Mathf.PI / 4))));
        this.boneObjects[0].GetComponent<SpriteRenderer>().color = color;

        for (int i = 1; i < numBones; i++) {
            this.boneObjects.Add(GameObject.Instantiate(bonePrefab, new Vector3(i - 18, 0, 0), new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 4), Mathf.Cos(Mathf.PI / 4))));
            this.boneObjects[i].GetComponent<SpriteRenderer>().color = color;
            CharacterJoint prevJoint = this.boneObjects[i - 1].GetComponent<CharacterJoint>();
            prevJoint.connectedBody = this.boneObjects[i].GetComponent<Rigidbody>();
        }
        CharacterJoint finalJoint = this.boneObjects[boneObjects.Count - 1].GetComponent<CharacterJoint>();
        Object.Destroy(finalJoint);

        // Make muscles
        for (int i = 0; i < this.boneObjects.Count; i++)
        {
            this.muscles.Add(new Muscle(this.boneObjects[i]));
        }

        for (int i = 0; i < this.boneObjects.Count; i++)
        {
            this.boneObjects[i].SetActive(true);
        }
    }

    public void ContractMuscles(List<float> contractions)
    {
        for (int i = 0; i < contractions.Count - 1; i++)
        {
            this.muscles[i].Contract(contractions[i]);
        }
    }

    public void zLock()
    {
        foreach (GameObject bone in this.boneObjects)
        {
            bone.transform.position = new Vector3(bone.transform.position[0], bone.transform.position[1], 0f);
            bone.transform.rotation = new Quaternion(bone.transform.rotation[0], 0f, bone.transform.rotation[2], bone.transform.rotation[3]);
        }
    }

    private List<T> SafeSubtractEntry<T>(List<T> list, T entry)
    {
        List<T> returnList = new List<T> {};
        for (int i = 0; i < list.Count; i++)
        {
            if (Object.Equals(list[i], entry)) {
                continue;
            }
            returnList.Add(list[i]);
        }
        return returnList;
    }

    public List<float> GetBasicInputs()
    {
        List<float> inputs = new List<float>{};
        foreach (GameObject bone in this.boneObjects)
        {
            inputs.Add(this.Sigmoid(bone.transform.position[0]));
            inputs.Add(this.Sigmoid(bone.transform.position[1]));
        }
        return inputs;
    }

    public List<float> GetProprioceptionInputs(BoxCollider groundCollider)
    {
        List<float> inputs = new List<float>{};
        foreach (GameObject bone in this.boneObjects)
        {
            inputs.Add(this.Sigmoid(2*Mathf.Acos(bone.transform.rotation[0])));
            if (groundCollider.bounds.Contains(bone.transform.position)) {
                inputs.Add(1f);
            } else {
                inputs.Add(0f);
            }
        }
        return inputs;
    }

    public float GetLeadPos()
    {
        float leadPos = this.boneObjects[this.boneObjects.Count - 1].transform.position[0];
        return leadPos;
    }

    public bool HasFinished()
    {
        if (this.finisher) {
            return true;
        }

        if (this.boneObjects[0].transform.position[0] > 18) {
            this.finisher = true;
            return true;
        }

        return false;
    }

    public void Delete()
    {
        foreach (GameObject bone in this.boneObjects)
        {
            GameObject.Destroy(bone);
        }
    }

    public float Sigmoid(float value) // 'Squeezes' x values so they lie between -1 and 1
    {
        return 1/(1+Mathf.Exp(-value)); // https://en.wikipedia.org/wiki/Sigmoid_function 
    }
}