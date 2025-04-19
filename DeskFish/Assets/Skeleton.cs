using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Skeleton
{
    List<Bone> bones = new List<Bone>(){};
    public List<Joint> joints = new List<Joint>(){};

    public Skeleton(int numJoints)
    {
        float boneLengths = 5f/numJoints;
        for (int i = 0; i < numJoints + 2; i++)
        {
            this.joints.Add(new Joint());
        }

        for (int i = 0; i < numJoints + 1; i++)
        {
            this.bones.Add(new Bone(this.joints[i], this.joints[i+1], boneLengths));
        }

        this.joints[0].AddBones(this.bones[0], true);
        this.joints[numJoints + 1].AddBones(this.bones[numJoints], false);
        this.joints[numJoints + 1].ShiftBy(new Vector3(1, 0, 0), false);
        

        for (int i = 1; i < numJoints + 1; i++)
        {
            this.joints[i].AddBones(this.bones[i - 1], this.bones[i]);
            this.joints[i].ShiftBy(new Vector3(i, 0, 0), false);
        }

        for (int i = 0; i < 10; i++)
        {
            this.UpdateSkeleton();
        }
    }
    
    public void UpdateSkeleton()
    {
        foreach (var bone in this.bones)
        {
            bone.LimitLength();
            bone.SetDescription();
        }
        foreach (var joint in this.joints)
        {
            joint.LimitRotation();
        }
    }

    public void MoveSkeleton(Vector3 movement)
    {
        this.joints[0].ShiftBy(movement, true);
    }
}
