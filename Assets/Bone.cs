using UnityEngine;
using System;
using System.Collections.Generic;

public class Bone
{
    // -- Necessary Information to be Carried --

    // Central Node Position
    // Angle Between Bone "Arms" 
    // Number of Active "Arms"
    // Length of "Arms"

    public Vector3 pos = new Vector3(0, 0, 0);
    public Quaternion rotation = new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 4), Mathf.Cos(Mathf.PI / 4));
    public int primacy;
    public List<Joint> joints = new List<Joint>{};
    public Vector3 primaryBone;

    private bool[] activeArms = new bool[]{true, true, false, false};
    public float[] armLengths = new float[]{0.5f, 0.5f, 0f, 0f};
    public Quaternion[] armAngles = new Quaternion[]{Quaternion.identity, new Quaternion(0, 0, Mathf.Sin(Mathf.PI / 2), Mathf.Cos(Mathf.PI / 2)), Quaternion.identity, Quaternion.identity};

    public float mass = 2f;
    public Vector3 forces = new Vector3(0, 0, 0);
    public Vector3 velocity = new Vector3(0, 0, 0);

    public Bone(Joint baseJoint)
    {
        this.pos = baseJoint.pos + new Vector3(this.armLengths[0], 0, 0);
        this.primacy = baseJoint.primacy;
        this.joints.Add(baseJoint);
        this.primaryBone = baseJoint.pos - this.pos;
    }

    public Bone(Joint baseJoint, int sign)
    {
        this.pos = baseJoint.pos + new Vector3(this.armLengths[0] * sign, 0, 0);
        this.primacy = baseJoint.primacy;
        this.joints.Add(baseJoint);
        this.primaryBone = baseJoint.pos - this.pos;
    }

    public Bone(Joint baseJoint, bool[] activeArms, float[] armLens, Quaternion[] armAngles)
    {
        this.pos = baseJoint.pos + new Vector3(armLens[0], 0, 0);
        this.primacy = baseJoint.primacy;
        this.joints.Add(baseJoint);
        this.activeArms = activeArms;
        this.armLengths = armLens;
        this.armAngles = armAngles;
    }

    public void AddForce(Vector3 force)
    {
        this.forces += force;
    }

    public void ResetForce()
    {
        this.forces = new Vector3(0, 0, 0);
    }

    public void Accelerate(float deltaTime)
    {
        Vector3 acceleration = this.forces / this.mass;
        this.velocity += acceleration * deltaTime;
    }

    public void ResolveVelocity(float deltaTime)
    {
        this.pos += velocity * deltaTime;
    }
}