using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Bone> bones = new List<Bone> {};
    private List<Joint> joints = new List<Joint> {};

    private List<GameObject> jointObjects = new List<GameObject> {};

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numJoints, float flexibility, float boneLen)
    {
        for (int i = 0; i < numJoints; i++) {
            bones.Add(new Bone(boneLen));
        }

        joints.Add(new Joint(flexibility, bones[0], bones[0], true, 0, 0)); // Origin joint, "Head"

        for (int i = 0; i < numJoints - 1; i++) {
            joints.Add(new Joint(flexibility, bones[i], bones[i + 1], false, -(boneLen * (i + 1)), 0));
        }

        joints.Add(new Joint(flexibility, bones[numJoints - 1], bones[numJoints - 1], true, -(boneLen * numJoints), 0)); // Terminal joint, "Tail"
    }

    public void UpdateSkeleton()
    {
        joints[0].pos += new Vector2(0.01f, 0f); // Shifts the origin joint

        for (int i = 0; i < 10; i++) {
            for (int j = 1; j < joints.Count; j++) {
                joints[j].CorrectPosition(joints[j - 1]);
            }
        }
    }

    public void InitVisualisation(GameObject jointPrefab)
    {
        for (int i = 0; i < joints.Count; i++) {
            this.jointObjects.Add(GameObject.Instantiate(jointPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        }

        this.UpdateVisualisation();
    }

    public void UpdateVisualisation()
    {
        for (int i = 0; i < joints.Count; i++) {
            jointObjects[i].transform.SetPositionAndRotation(joints[i].pos, Quaternion.identity);
            jointObjects[i].SetActive(true);
        }
    }
}
