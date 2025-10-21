using UnityEngine;
using System.Collections.Generic;

public class Skeleton
{
    private List<Bone> bones = new List<Bone> {};
    private List<Joint> joints = new List<Joint> {};

    // Store the joints and bones of the skeleton of a given organism
    public Skeleton(int numJoints, float flexibility, float boneLen)
    {
        for (int i = 0; i < numJoints - 1; i++) {
            bones.Add(new Bone(boneLen));
        }

        joints.Add(new Joint(flexibility, bones[0], bones[0], true, 0, 0)); // Origin joint, "Head"

        for (int i = 0; i < numJoints - 2; i++) {
            joints.Add(new Joint(flexibility, bones[i], bones[i + 1], false, boneLen * i, 0));
        }

        joints.Add(new Joint(flexibility, bones[numJoints - 1], bones[numJoints - 1], true, boneLen * numJoints, 0)); // Terminal joint, "Tail"
    }
}
