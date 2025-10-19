using UnityEngine;

public class Joint
{
    private float flexibility;
    private Bone boneOne;
    private Bone boneTwo;
    private bool isTerminus;
    
    public Joint(float flexibility, Bone boneOne, Bone boneTwo, bool isTerminus)
    {
        self.flexibility = flexibility; // rotational freedom in radians
        self.boneOne = boneOne;
        
        self.isTerminus = isTerminus;

        if (!isTerminus) {
            self.boneTwo = boneTwo
        }
    }
}
