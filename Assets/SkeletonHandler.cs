using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private float flexibility;
    [SerializeField] private float boneLen;

    private Skeleton skeleton;

    void Start()
    {
       skeleton = new Skeleton(7, flexibility, boneLen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
