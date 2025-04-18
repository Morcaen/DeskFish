using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Joint
{
    public Vector3 pos = new Vector3();
    Bone[] bones = new Bone[2];
    float rotFreedom = Convert.ToSingle((4/3)*Math.PI);
    bool terminus = false;

    public void AddBones(Bone boneOne, Bone boneTwo)
    {
        this.bones[0] = boneOne;
        this.bones[1] = boneTwo;
    }

    public void AddBones(Bone boneOne)
    {
        this.bones = new Bone[1];
        this.bones[0] = boneOne;
    }

    public void LimitRotation()
    {
        if (this.terminus) {
            return;
        }
        // Find joint angle
        Vector3 normalizedBoneDescOne = Vector3.Normalize(this.bones[0].description);
        if (this.bones[0].joints[0] == this) {
            normalizedBoneDescOne = -normalizedBoneDescOne;
        }
        Vector3 normalizedBoneDescTwo = Vector3.Normalize(this.bones[1].description);
        if (this.bones[1].joints[1] == this) {
            normalizedBoneDescTwo = -normalizedBoneDescTwo;
        }
        float jointAngle = (float)Math.Acos(Vector3.Dot(normalizedBoneDescOne, normalizedBoneDescTwo));
        
        if (jointAngle > 0.5*this.rotFreedom) {
            return;
        }
        
        float degreeOfError = (float)0.5*this.rotFreedom - jointAngle;
        Vector3 normalVector = Vector3.Cross(normalizedBoneDescOne, normalizedBoneDescTwo);

        this.bones[0].RotateBy(0.5f*degreeOfError, normalVector);
        this.bones[1].RotateBy(-0.5f*degreeOfError, normalVector);
    }

    public void ShiftBy(Vector3 shift)
    {
        this.pos += shift;
        for (int i = 0; i < this.bones.Length; i++) {
            this.bones[i].SetDescription();
        }
    }
}