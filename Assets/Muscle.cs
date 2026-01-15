using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Muscle
{
    private float maximumContractualForce = 10; // Newtons
    private GameObject baseBone;

    public Muscle(GameObject baseBone)
    {
        this.baseBone = baseBone;
    }

    public void Contract(float contraction)
    {
        contraction = Mathf.Clamp(contraction, -1f, 1f);
        float contractionMagnitude = contraction * maximumContractualForce;

        Rigidbody baseRB = this.baseBone.GetComponent<Rigidbody>();
        Rigidbody connRB = this.baseBone.GetComponent<CharacterJoint>().connectedBody;
        baseRB.AddRelativeTorque(new Vector3(0, 0, 1) * contractionMagnitude);
        connRB.AddRelativeTorque(new Vector3(0, 0, -1) * contractionMagnitude);
    }
}
