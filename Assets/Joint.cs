using UnityEngine;
using System;

public class Joint
{
    public Vector2 pos = new Vector2(0, 0);

    private float flexibility = 0f;
    private Bone boneOne;
    private Bone boneTwo;
    private bool isTerminus;

    public Joint(float flexibility, Bone boneOne, Bone boneTwo, bool isTerminus, float xOffset, float yOffset)
    {
        this.flexibility = flexibility; // rotational freedom in radians
        this.boneOne = boneOne;
        
        this.isTerminus = isTerminus;

        if (!isTerminus) {
            this.boneTwo = boneTwo;
        }

        this.pos += new Vector2(xOffset, yOffset);
    }

    public void CorrectPosition(Joint neighbouringJoint)
    {
        float targetSeperation = boneOne.len;

        Vector2 seperationVector = neighbouringJoint.pos - this.pos; // Resultant vector points from this joint to the neighbouring joint
        float actualSeperation = seperationVector.magnitude;

        if (actualSeperation < 0.95*targetSeperation || actualSeperation > 1.05*targetSeperation) {
            float error = actualSeperation - targetSeperation; // If the joints are too close, this value will be negative
            Vector2 direction = seperationVector.normalized;

            this.pos += 0.5f * error * direction;
            neighbouringJoint.pos += -0.5f * error * direction;
        }
    }
}
