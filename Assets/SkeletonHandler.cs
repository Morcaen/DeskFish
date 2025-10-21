using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private float flexibility;
    [SerializeField] private float boneLen;
    [SerializeField] private GameObject jointPrefab;

    private Skeleton skeleton;

    void Start()
    {
       skeleton = new Skeleton(7, flexibility, boneLen);
       skeleton.InitVisualisation(jointPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        skeleton.UpdateVisualisation();
    }
}
