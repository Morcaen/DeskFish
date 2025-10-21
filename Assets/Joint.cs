using UnityEngine;

public class Joint
{
    private float flexibility = 0f;
    private float xPos = 0f;
    private float yPos = 0f;
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

        this.xPos += xOffset;
        this.yPos += yOffset;
    }
}
