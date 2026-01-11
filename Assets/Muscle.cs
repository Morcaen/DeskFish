using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Muscle
{
    private float maximumContractualForce = 20; // Newtons
    private List<Joint> proxJoints;
    private Joint baseJoint;
    private List<Joint> distJoints;

    private float sign;

    public Muscle(List<Joint> proxJoints, Joint baseJoint, List<Joint> distJoints, float sign)
    {
        this.proxJoints = proxJoints;
        this.baseJoint = baseJoint;
        this.distJoints = distJoints;

        this.sign = sign;
    }

    public void Contract(float contraction)
    {
        contraction = Mathf.Clamp(contraction, 0f, 1f);
        float contractionMagnitude = contraction * maximumContractualForce * this.sign;

        for (int i = 0; i < this.proxJoints.Count; i++)
        {
            Vector3 baseToJointVec = this.proxJoints[i].pos - this.baseJoint.pos;
            Vector3 contractionVec = new Vector3(baseToJointVec[1], -baseToJointVec[0], baseToJointVec[2]); // Vector orthogonal to the base to joint vec
            contractionVec.Normalize();

            Vector3 contractionScalar = contractionVec * contractionMagnitude;
            this.proxJoints[i].AddForce(contractionScalar);
            this.baseJoint.AddForce(-contractionScalar);
        }

        for (int i = 0; i < this.distJoints.Count; i++)
        {
            Vector3 baseToJointVec = this.distJoints[i].pos - this.baseJoint.pos;
            Vector3 contractionVec = new Vector3(-baseToJointVec[1], baseToJointVec[0], baseToJointVec[2]); // Vector orthogonal to the base to joint vec
            contractionVec.Normalize();

            Vector3 contractionScalar = contractionVec * contractionMagnitude;
            this.distJoints[i].AddForce(contractionScalar);
            this.baseJoint.AddForce(-contractionScalar);
        }
    }
}
