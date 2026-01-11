using UnityEngine;
using System;

public class Joint
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public int primacy;
    public float mass = 1f;
    
    public Vector3 forces = new Vector3(0, 0 ,0);
    public Vector3 velocity = new Vector3(0, 0, 0);
    private float flexibility = 0f;

    public Bone baseBone;
    public Bone contBone;

    public Joint(float flexibility, Bone baseBone)
    {
        this.flexibility = flexibility; // rotational freedom in radians

        this.primacy = baseBone.primacy + 1;
        baseBone.joints.Add(this);

        this.baseBone = baseBone;

        Quaternion rotation = baseBone.armAngles[1];
        Quaternion boneQuat = rotation * new Quaternion(baseBone.primaryBone[0], baseBone.primaryBone[1], baseBone.primaryBone[2], 0) * Quaternion.Inverse(rotation);
        Vector3 boneVec = new Vector3(boneQuat[0], boneQuat[1], boneQuat[2]);
        boneVec.Normalize();
        boneVec = boneVec * baseBone.armLengths[1];
        this.pos = boneVec + baseBone.pos;
    }

    // Overload for primary joint
    public Joint(float xOffset, float yOffset)
    {
        this.pos += new Vector3(xOffset, yOffset, 0);
        this.primacy = 1;
    }

    public void AssignBone(Bone addBone)
    {
        this.contBone = addBone;
    }

    public void AssignBone(Bone boneOne, Bone boneTwo)
    {
        this.baseBone = boneOne;
        this.contBone = boneTwo;
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
