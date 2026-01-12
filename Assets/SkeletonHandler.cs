using UnityEngine;

public class SkeletonHandler : MonoBehaviour
{
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private GameObject bonePrefab;
    [SerializeField] private float gravAcc = -9.8f;
    [SerializeField] private BoxCollider2D groundCollider;

    private Skeleton skeleton;

    void Start()
    {
       skeleton = new Skeleton(7, Mathf.PI/8f);
       skeleton.InitVisualisation(jointPrefab, bonePrefab);
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
        skeleton.Friction(groundCollider);
        skeleton.NormalForce(groundCollider);

        // Resolve Forces
        skeleton.ResloveForces(Time.deltaTime);

        // Update Visualisation
        skeleton.UpdateJointVisualisation();
        skeleton.UpdateBoneVisualisation();
    }
}
