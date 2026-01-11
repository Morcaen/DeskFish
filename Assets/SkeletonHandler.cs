using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private float flexibility;
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private float gravAcc = -9.8f;
    [SerializeField] private BoxCollider2D groundCollider;

    private Skeleton skeleton;

    void Start()
    {
       skeleton = new Skeleton(7, flexibility);
       skeleton.InitJointVisualisation(jointPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate Forces
        skeleton.ResetForces();
        if (Input.GetKey(KeyCode.Space)) {
            skeleton.TestContraction();
        }
        skeleton.Gravitate(gravAcc);
        skeleton.NormalForce(groundCollider);

        // Resolve Forces
        skeleton.ResloveForces(Time.deltaTime);

        // Update Visualisation
        skeleton.UpdateJointVisualisation();
    }
}
