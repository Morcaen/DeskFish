using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Bone> bones = new List<Bone> {};
    private List<Joint> joints = new List<Joint> {};
    private List<Muscle> muscles = new List<Muscle> {};
    private Joint primaryJoint;

    private List<GameObject> jointObjects = new List<GameObject> {};

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numJoints, float flexibility)
    {
        // Make joints and bones
        this.primaryJoint = new Joint(0f, 0f);
        this.joints.Add(primaryJoint);
        this.bones.Add(new Bone(primaryJoint, 1));
        this.bones.Add(new Bone(primaryJoint, -1));
        this.joints[0].AssignBone(this.bones[0], this.bones[1]);

        for (int i = 1; i < numJoints; i++) {
            int sign = 1;
            if (i%2 == 0){
                sign = -1;
            }
            this.joints.Add(new Joint(flexibility, this.bones[i - 1]));
            this.bones.Add(new Bone(this.joints[i], sign));
            this.joints[this.joints.Count - 1].AssignBone(this.bones[this.bones.Count - 1]);
        }

        // Make muscles
        for (int i = 0; i < this.joints.Count; i++)
        {
            Joint baseJoint = this.joints[i];
            if (baseJoint.contBone.joints.Count == 1) { // If terminus
                continue;
            }
            List<Joint> basicJoints = this.SafeSubtractEntry<Joint>(baseJoint.baseBone.joints, baseJoint);
            Debug.Log(basicJoints.Count + " proximal joints");
            List<Joint> distalJoints = this.SafeSubtractEntry<Joint>(baseJoint.contBone.joints, baseJoint);
            Debug.Log("And " + distalJoints.Count + " distal joints");

            // Create a protagonist and antagonist muscle between its two neighbours
            this.muscles.Add(new Muscle(basicJoints, baseJoint, distalJoints, 1f));
            this.muscles.Add(new Muscle(basicJoints, baseJoint, distalJoints, -1f));
        }
    }

    public void TestContraction()
    {
        this.muscles[4].Contract(1f);
    }


    // CALCULATE FORCES
    public void ResetForces()
    {
        foreach (Joint joint in this.joints) {
            joint.ResetForce();
        }
    }

    public void Gravitate(float gravAcc)
    {
        foreach (Joint joint in this.joints) {
            joint.AddForce(new Vector3(0, joint.mass * gravAcc, 0));
        }
    }

    public void NormalForce(BoxCollider2D groundCollider)
    {
        foreach (Joint joint in this.joints) {
            if (!groundCollider.bounds.Contains(joint.pos - new Vector3(0, 0.4f, 0))) {
                continue;
            }
            if (joint.forces[1] < 0f) {
                joint.forces[1] = 0f;
            }
            if (joint.velocity[1] < 0f) {
                joint.velocity[1] = 0f;
            }
        }
    }

    // RESOLVE FORCES
    public void ResloveForces(float deltaTime)
    {
        foreach (Joint joint in this.joints) {
            joint.Accelerate(deltaTime);
            joint.ResolveVelocity(deltaTime);
        }
    }

    // VISUALISATION
    public void InitJointVisualisation(GameObject jointPrefab)
    {
        for (int i = 0; i < joints.Count; i++) {
            this.jointObjects.Add(GameObject.Instantiate(jointPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        }

        this.UpdateJointVisualisation();
    }

    public void UpdateJointVisualisation()
    {
        for (int i = 0; i < joints.Count; i++) {
            jointObjects[i].transform.SetPositionAndRotation(joints[i].pos, Quaternion.identity);
            jointObjects[i].SetActive(true);
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
