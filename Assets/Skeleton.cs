using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Muscle> muscles = new List<Muscle> {};

    private List<GameObject> boneObjects = new List<GameObject> {};

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numBones, GameObject bonePrefab)
    {
        // Make joints and bones
        this.boneObjects.Add(GameObject.Instantiate(bonePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 4), Mathf.Cos(Mathf.PI / 4))));

        for (int i = 1; i < numBones; i++) {
            this.boneObjects.Add(GameObject.Instantiate(bonePrefab, new Vector3(i, 0, 0), new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 4), Mathf.Cos(Mathf.PI / 4))));
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

    public void TestContraction(int muscle)
    {
        this.muscles[muscle].Contract(1f);
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
}