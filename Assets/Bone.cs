using UnityEngine;

public class Bone
{
    private float len;
    private Joint jointOne;
    private Joint jointTwo;

    public Bone(float len, Joint jointOne, Joint jointTwo)
    {
        self.len = len;
        
        self.jointOne = jointOne;
        self.jointTwo = jointTwo;
    }
}
