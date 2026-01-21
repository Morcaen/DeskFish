using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Muscle
{
    private float maximumContractualForce = 15; // Newtons
    private GameObject baseBone;

    public Muscle(GameObject baseBone)
    {
        this.baseBone = baseBone;
    }

    public void Contract(float contraction)
    {   // This system must change, rather than contraction simply being AddTorque it should instead target a rotation of the specified proportion of maximal rotation
        contraction = Mathf.Clamp(contraction, 0f, 1f);

        Rigidbody baseRB = this.baseBone.GetComponent<Rigidbody>();
        CharacterJoint joint = this.baseBone.GetComponent<CharacterJoint>();
        Rigidbody connRB = joint.connectedBody;

        float targetAngle = 70f * contraction;
        float angle = Quaternion.Angle(baseRB.rotation, connRB.rotation);

        float realVsTarget = targetAngle - angle;
        float invPropTarget = realVsTarget / 70f;

        baseRB.AddRelativeTorque(new Vector3(0, 0, 1) * maximumContractualForce * invPropTarget);
        connRB.AddRelativeTorque(new Vector3(0, 0, -1) * maximumContractualForce * invPropTarget);
    }
}
