using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Bone> bones = new List<Bone> {};
    private List<Joint> joints = new List<Joint> {};
    private Joint primaryJoint;

    private List<GameObject> jointObjects = new List<GameObject> {};

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numJoints, float flexibility)
    {
        this.primaryJoint = new Joint(0f, 0f);
        this.joints.Add(primaryJoint);
        this.bones.Add(new Bone(primaryJoint, 1));
        this.bones.Add(new Bone(primaryJoint, -1));

        for (int i = 1; i < numJoints; i++) {
            int sign = 1;
            if (i%2 == 0){
                sign = -1;
            }
            this.joints.Add(new Joint(flexibility, this.bones[i - 1]));
            this.bones.Add(new Bone(this.joints[i], sign));
        }
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
                return;
            }
            joint.forces[1] = 0f;
            joint.velocity[1] = 0f;
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
}
