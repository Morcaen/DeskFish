using System.Collections.Generic;
using System;
using UnityEngine;

public class Bone
{
    public Vector3 description = new Vector3();
    float targetLength;
    public Joint[] joints = new Joint[2];

    public Bone(Joint jointOne, Joint jointTwo, float targetLength)
    {
        this.joints[0] = jointOne;
        this.joints[1] = jointTwo;
        this.targetLength = targetLength;
    }

    public void SetDescription()
    {
        this.description = this.joints[0].pos - this.joints[1].pos;
    }

    public void RotateBy(float theta, Vector3 axis)
    {
        float w = (float)Math.Cos(theta/2);
        Quaternion rotation = new Quaternion(w, axis.x, axis.y, axis.z);
        this.description = rotation * this.description;
    }

    public void LimitLength()
    {
        float sqrMagnitude = this.description.sqrMagnitude;
        if (this.Within(sqrMagnitude, 0.1f, this.targetLength*this.targetLength)) { // Within error bounds, do nothing
            return;
        }
        float magnitude = (float)Math.Sqrt(sqrMagnitude);
        float error = this.targetLength - magnitude;
        if (this.joints[0].fixedJoint) {
            this.joints[1].ShiftBy(-this.description/magnitude*error, false);
            return;
        }
        this.joints[0].ShiftBy(this.description/magnitude*0.5f*error, false);
        this.joints[1].ShiftBy(-this.description/magnitude*0.5f*error, false);
    }

    bool Within(float value, float allowance, float target)
    {
        if (value < target + allowance && value > target - allowance) {
            return true;
        }
        return false;
    }

}
