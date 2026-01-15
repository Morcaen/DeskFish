using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private GameObject bonePrefab;

    private Skeleton skeleton;

    void Start()
    {
       skeleton = new Skeleton(7, bonePrefab);
    }

    // Update is called once per frame
    void Update()
    {
        skeleton.zLock();
        if (Input.GetKey(KeyCode.Space)) {
            skeleton.TestContraction(4);
        }
    }
}
