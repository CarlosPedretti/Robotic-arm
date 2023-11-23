using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Rigidbody baseJoint;
    public Rigidbody midJoint;
    public Rigidbody endEffector;

    public float rotationSpeed = 5f;

    void Update()
    {
        // Control de la base del brazo
        float baseRotation = Input.GetAxis("Horizontal") * rotationSpeed;
        baseJoint.AddTorque(Vector3.up * baseRotation);

        // Control de la articulación media del brazo
        float midRotation = Input.GetAxis("Vertical") * rotationSpeed;
        midJoint.AddTorque(Vector3.up * midRotation);

        // Control del efector final (punta del brazo)
        float endEffectorRotation = Input.GetAxis("Jump") * rotationSpeed;
        endEffector.AddTorque(Vector3.up * endEffectorRotation);
    }

}
